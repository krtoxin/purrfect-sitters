using System.Net;
using System.Net.Http.Json;
using Api.Contracts.Bookings;
using Api.DTOs;
using FluentAssertions;
using Tests.Common;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common.Services;
using Tests.Data.Bookings;
using Tests.Data.Pets;
using Tests.Data.Sitters;
using Tests.Data.Users;
using Application.Common.Interfaces;
using Xunit;

namespace Api.Tests.Integration.Bookings;

[Collection("Integration")]
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
        var sitterUser = UserData.CreateUser(); 
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);

        Context.Users.Add(owner);
        Context.Users.Add(sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var startUtc = DateTime.UtcNow.AddDays(1);
        var endUtc = startUtc.AddHours(2);

        var request = new
        {
            PetId = pet.Id,
            SitterProfileId = sitterProfile.Id,   
            StartUtc = startUtc,                 
            EndUtc = endUtc,
            BaseAmount = 50.00m,                  
            ServiceFeePercent = 10.0m,
            Currency = "USD",
            CareInstructionTexts = new[] { "Please take good care of my pet" }
        };

        var response = await Client.PostAsJsonAsync("/api/bookings", request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("==== SERVER RESPONSE START ====");
            Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(string.IsNullOrWhiteSpace(body) ? "<empty>" : body);
            Console.WriteLine("==== SERVER RESPONSE END ====");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<BookingDto>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task UpdateStatus_AcceptBooking_UpdatesStatus()
    {
        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var booking = BookingData.CreateBooking(ownerId: owner.Id, sitterProfileId: sitterProfile.Id);

        Context.Users.Add(owner);
        Context.Users.Add(sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Bookings.Add(booking);
        await SaveChangesAsync();

        var request = new UpdateBookingDto
        {
            Status = "Accepted",
            CareInstructions = ""
        };

        var response = await Client.PutAsJsonAsync($"/api/bookings/{booking.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedBooking = await response.Content.ReadFromJsonAsync<BookingDto>();
        updatedBooking.Should().NotBeNull();
        updatedBooking!.Status.Should().Be("Accepted");
    }

    [Fact]
    public async Task GetAllBookings_ReturnsBookings()
    {
        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var bookings = BookingData.CreateBookings(3, ownerId: owner.Id, sitterProfileId: sitterProfile.Id).ToList();

        Context.Users.Add(owner);
        Context.Users.Add(sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        var response = await Client.GetAsync("/api/bookings");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedBookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
        returnedBookings.Should().NotBeNull();
        returnedBookings!.Count.Should().Be(3);
        returnedBookings.Should().AllSatisfy(b => b.OwnerId.Should().Be(owner.Id));
    }
}