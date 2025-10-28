using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Bookings;
using FluentAssertions;
using Tests.Common;
using Tests.Common.Services;
using Tests.Data.Bookings;
using Tests.Data.Pets;
using Tests.Data.Sitters;
using Tests.Data.Users;
using Application.Common.Interfaces;
using Xunit;

namespace Api.Tests.Integration.Bookings;

public class BookingsControllerTests : BaseIntegrationTest
{
    private readonly IEmailSendingService _emailService;

    public BookingsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _emailService = factory.Services.GetRequiredService<IEmailSendingService>();
    }

    [Fact]
    public async Task Create_ValidBooking_ReturnsCreatedBooking()
    {
        var owner = UserData.CreateUser();
        var sitter = SitterData.CreateSitter();
        var pets = PetData.CreatePets(2, owner).ToList();
        
        Context.Users.Add(owner);
        Context.Sitters.Add(sitter);
        Context.Pets.AddRange(pets);
        await SaveChangesAsync();

        var request = new CreateBookingRequest(
            sitter.Id,
            pets.Select(p => p.Id).ToList(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            new AddressDto("123 Service St", "Apt 1", "Service City", "Service State", "12345", "Test Country"),
            "Please take good care of my pets"
        );

        var response = await Client.PostAsJsonAsync("/api/bookings", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdBooking = await response.Content.ReadFromJsonAsync<BookingResponse>();
        createdBooking.Should().NotBeNull();
        createdBooking!.SitterId.Should().Be(sitter.Id);
        createdBooking.Status.Should().Be(BookingStatus.Pending);

        var emailService = (InMemoryEmailService)_emailService;
        emailService.SentEmails.Should().ContainSingle();
        var sentEmail = emailService.SentEmails[0];
        sentEmail.To.Should().Be(sitter.Email);
        sentEmail.Subject.Should().Contain("New Booking Request");
    }

    [Fact]
    public async Task UpdateStatus_AcceptBooking_UpdatesStatusAndNotifiesOwner()
    {
        var owner = UserData.CreateUser();
        var sitter = SitterData.CreateSitter();
        var booking = BookingData.CreateBooking(owner: owner, sitter: sitter);
        
        Context.Users.Add(owner);
        Context.Sitters.Add(sitter);
        Context.Bookings.Add(booking);
        await SaveChangesAsync();

        var request = new UpdateBookingStatusRequest(BookingStatus.Accepted);

        var response = await Client.PutAsJsonAsync($"/api/bookings/{booking.Id}/status", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedBooking = await response.Content.ReadFromJsonAsync<BookingResponse>();
        updatedBooking.Should().NotBeNull();
        updatedBooking!.Status.Should().Be(BookingStatus.Accepted);

        var emailService = (InMemoryEmailService)_emailService;
        emailService.SentEmails.Should().ContainSingle();
        var sentEmail = emailService.SentEmails[0];
        sentEmail.To.Should().Be(owner.Email);
        sentEmail.Subject.Should().Contain("Booking Accepted");
    }

    [Fact]
    public async Task GetUserBookings_ReturnsUserBookings()
    {
        var owner = UserData.CreateUser();
        var sitter = SitterData.CreateSitter();
        var bookings = BookingData.CreateBookings(3, owner: owner, sitter: sitter).ToList();
        
        Context.Users.Add(owner);
        Context.Sitters.Add(sitter);
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/bookings/my");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedBookings = await response.Content.ReadFromJsonAsync<List<BookingResponse>>();
        returnedBookings.Should().NotBeNull();
        returnedBookings!.Count.Should().Be(3);
        returnedBookings.Should().AllSatisfy(b => b.OwnerId.Should().Be(owner.Id));
    }
}