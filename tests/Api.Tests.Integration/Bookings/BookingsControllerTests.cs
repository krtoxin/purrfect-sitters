using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Bookings;
using Tests.Data.Pets;
using Tests.Data.Sitters;
using Tests.Data.Users;
using Xunit;

namespace Api.Tests.Integration.Bookings;

[Collection("Integration")]
public class BookingsControllerTests : BaseIntegrationTest
{
    public BookingsControllerTests(IntegrationTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_ValidBooking_ReturnsCreatedBooking()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.Pets.RemoveRange(Context.Pets);
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);

        Context.Users.Add(owner);
        Context.Users.Add(sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var createRequest = new
        {
            PetId = pet.Id,
            SitterProfileId = sitterProfile.Id,
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(1).AddHours(2),
            BaseAmount = 50.00m,
            ServiceFeePercent = 10.0m,
            Currency = "USD",
            CareInstructionTexts = new[] { "Please take good care of my pet" }
        };

    // Act
    var createResponse = await Client.PostAsJsonAsync("/api/bookings", createRequest);
        
    // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        createdObj.Should().NotBeNull();
        var idRaw = createdObj["id"].ToString();
        Guid bookingId = Guid.Parse(idRaw!);

    // Act
        var getResponse = await Client.GetAsync($"/api/bookings/{bookingId}");
        
    // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var booking = await getResponse.Content.ReadFromJsonAsync<BookingDto>();
        booking.Should().NotBeNull();

        Context.ChangeTracker.Clear();
        var dbBookingBefore = await Context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookingId);
        var dbXminBefore = dbBookingBefore != null ? Context.Entry(dbBookingBefore).Property("xmin").CurrentValue : null;
        Console.WriteLine($"[TEST] Booking before accept (API): {System.Text.Json.JsonSerializer.Serialize(booking)}");
        Console.WriteLine($"[TEST] Booking before accept (DB): {System.Text.Json.JsonSerializer.Serialize(dbBookingBefore)} xmin={dbXminBefore}");
        
        // ASSERT
        booking!.Status.Should().Be("Requested", "Початковий статус бронювання має бути Requested.");

    // Act
    var sitterClient = GetAuthenticatedClient(sitterUser.Id);
    var acceptResponse = await sitterClient.PostAsync($"/api/bookings/{bookingId}/accept", null);
        
        if (acceptResponse.StatusCode != HttpStatusCode.NoContent)
        {
            var acceptBody = await acceptResponse.Content.ReadAsStringAsync();
            Console.WriteLine("==== ACCEPT RESPONSE START ====");
            Console.WriteLine($"Status: {(int)acceptResponse.StatusCode} {acceptResponse.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(string.IsNullOrWhiteSpace(acceptBody) ? "<empty>" : acceptBody);
            Console.WriteLine("==== ACCEPT RESPONSE END ====");
        }
    // Assert
    acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    // Act
    var getResponse2 = await Client.GetAsync($"/api/bookings/{bookingId}");
        getResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await getResponse2.Content.ReadFromJsonAsync<BookingDto>();
        updated.Should().NotBeNull();

        Context.ChangeTracker.Clear();
        var dbBookingAfter = await Context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookingId);
        var dbXminAfter = dbBookingAfter != null ? Context.Entry(dbBookingAfter).Property("xmin").CurrentValue : null;
        Console.WriteLine($"[TEST] Booking after accept (API): {System.Text.Json.JsonSerializer.Serialize(updated)}");
        Console.WriteLine($"[TEST] Booking after accept (DB): {System.Text.Json.JsonSerializer.Serialize(dbBookingAfter)} xmin={dbXminAfter}");

    // Assert
    updated!.Status.Should().Be("Accepted", "Статус бронювання в API повинен бути 'Accepted' після успішного виклику.");
        
        var dbStatus = dbBookingAfter != null ? dbBookingAfter.Status.ToString() : "<not found>";
        dbStatus.Should().Be("Accepted", "Статус бронювання в БД повинен бути 'Accepted' після успішного виклику Accept.");
    }

    [Fact]
    public async Task GetAllBookings_ReturnsBookings()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.Pets.RemoveRange(Context.Pets);
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var bookings = BookingData.CreateBookings(3, ownerId: owner.Id, sitterProfileId: sitterProfile.Id).ToList();

        Context.Users.Add(owner);
        Context.Users.Add(sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/bookings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedBookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
        returnedBookings.Should().NotBeNull();
        returnedBookings!.Count.Should().Be(3);
        returnedBookings.Should().AllSatisfy(b => b.OwnerId.Should().Be(owner.Id));
    }

    
    private class PagedBookings
    {
        public List<BookingDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    [Fact]
    public async Task ListForOwner_ReturnsPagedResults()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.Pets.RemoveRange(Context.Pets);
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);
        Context.Users.AddRange(owner, sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var bookings = BookingData.CreateBookings(3, ownerId: owner.Id, sitterProfileId: sitterProfile.Id, petId: pet.Id).ToList();
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/bookings/owner/{owner.Id}?page=1&pageSize=2");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedBookings>();
        paged.Should().NotBeNull();
        paged!.Items.Count.Should().BeLessThanOrEqualTo(2);
        paged.Items.Should().OnlyContain(b => b.OwnerId == owner.Id);
        paged.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        paged.Page.Should().Be(1);
    }

    [Fact]
    public async Task ListForOwner_Page2_ReturnsRemaining()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.Pets.RemoveRange(Context.Pets);
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);
        Context.Users.AddRange(owner, sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var bookings = BookingData.CreateBookings(3, ownerId: owner.Id, sitterProfileId: sitterProfile.Id, petId: pet.Id).ToList();
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/bookings/owner/{owner.Id}?page=2&pageSize=2");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedBookings>();
        paged.Should().NotBeNull();
        paged!.Items.Count.Should().Be(1);
        paged.Items.Should().OnlyContain(b => b.OwnerId == owner.Id);
        paged.Page.Should().Be(2);
    }


    

    [Fact]
    public async Task ProblemDetails_422_HasCorrelationIdAndContentType()
    {
        // Arrange
        var badRequest = new
        {
            PetId = Guid.Empty,
            SitterProfileId = Guid.Empty,
            StartUtc = DateTime.MinValue,
            EndUtc = DateTime.MinValue,
            BaseAmount = -1m,
            ServiceFeePercent = -5m,
            Currency = "",
            CareInstructionTexts = Array.Empty<string>()
        };
        // Act
        var response = await Client.PostAsJsonAsync("/api/bookings", badRequest);
        // Assert
        response.StatusCode.Should().Be((HttpStatusCode)422);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType!.MediaType.Should().Contain("json");
        response.Headers.Contains("X-Correlation-Id").Should().BeTrue();
    }

    [Fact]
    public async Task ListForOwner_PageBeyondTotal_ReturnsEmpty()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        Context.Pets.RemoveRange(Context.Pets);
        Context.SitterProfiles.RemoveRange(Context.SitterProfiles);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);
        Context.Users.AddRange(owner, sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var bookings = BookingData.CreateBookings(2, ownerId: owner.Id, sitterProfileId: sitterProfile.Id, petId: pet.Id).ToList();
        Context.Bookings.AddRange(bookings);
        await SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/bookings/owner/{owner.Id}?page=5&pageSize=2");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedBookings>();
        paged.Should().NotBeNull();
        paged!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_InvalidBooking_ReturnsUnprocessableEntity()
    {
        // Arrange
        var badRequest = new
        {
            PetId = Guid.Empty,
            SitterProfileId = Guid.Empty,
            StartUtc = DateTime.MinValue,
            EndUtc = DateTime.MinValue,
            BaseAmount = -1m,
            ServiceFeePercent = -5m,
            Currency = "",
            CareInstructionTexts = Array.Empty<string>()
        };
        // Act
        var response = await Client.PostAsJsonAsync("/api/bookings", badRequest);
        // Assert
        response.StatusCode.Should().Be((HttpStatusCode)422);
    }

    [Fact]
    public async Task Update_NonExistingBooking_ReturnsNotFound()
    {
        // Arrange/Act
        var response = await Client.PutAsJsonAsync($"/api/bookings/{Guid.NewGuid()}", new { Status = "Accepted" });
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        // Arrange/Act
        var response = await Client.GetAsync($"/api/bookings/{Guid.NewGuid()}");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ExistingBooking_RemovesBooking()
    {
        // Arrange
        Context.Bookings.RemoveRange(Context.Bookings);
        await SaveChangesAsync();

        var owner = UserData.CreateUser();
        var sitterUser = UserData.CreateUser();
        var sitterProfile = SitterData.CreateSitterProfile(userId: sitterUser.Id);
        var pet = PetData.FirstPet(owner.Id);
        Context.Users.AddRange(owner, sitterUser);
        Context.SitterProfiles.Add(sitterProfile);
        Context.Pets.Add(pet);
        await SaveChangesAsync();

        var createRequest = new
        {
            PetId = pet.Id,
            SitterProfileId = sitterProfile.Id,
            StartUtc = DateTime.UtcNow.AddDays(1),
            EndUtc = DateTime.UtcNow.AddDays(1).AddHours(1),
            BaseAmount = 20.0m,
            ServiceFeePercent = 10.0m,
            Currency = "USD",
            CareInstructionTexts = new[] { "Care" }
        };
        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/bookings", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var bookingId = Guid.Parse(createdObj!["id"].ToString()!);

        // Act
        var delResponse = await Client.DeleteAsync($"/api/bookings/{bookingId}");
        // Assert
        delResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var exists = await Context.Bookings.AnyAsync(b => b.Id == bookingId);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_NonExistingBooking_ReturnsNotFound()
    {
        // Arrange/Act
        var response = await Client.DeleteAsync($"/api/bookings/{Guid.NewGuid()}");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    protected HttpClient CreateClientAs(Guid userId)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-User-Id", userId.ToString());
        return client;
    }
    private HttpClient GetAuthenticatedClient(Guid userId)
    {
        var client = Factory.CreateClient(); 
        client.DefaultRequestHeaders.Add("X-Test-User-Id", userId.ToString());
        return client;
    }
    
}