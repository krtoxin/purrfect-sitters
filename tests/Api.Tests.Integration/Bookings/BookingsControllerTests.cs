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

        var createResponse = await Client.PostAsJsonAsync("/api/bookings", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdObj = await createResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        createdObj.Should().NotBeNull();
        var idRaw = createdObj["id"].ToString();
        Guid bookingId = Guid.Parse(idRaw!);

        var getResponse = await Client.GetAsync($"/api/bookings/{bookingId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var booking = await getResponse.Content.ReadFromJsonAsync<BookingDto>();
        booking.Should().NotBeNull();

        var dbBookingBefore = await Context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        var dbXminBefore = dbBookingBefore != null ? Context.Entry(dbBookingBefore).Property("xmin").CurrentValue : null;
        Console.WriteLine($"[TEST] Booking before accept (API): {System.Text.Json.JsonSerializer.Serialize(booking)}");
        Console.WriteLine($"[TEST] Booking before accept (DB): {System.Text.Json.JsonSerializer.Serialize(dbBookingBefore)} xmin={dbXminBefore}");

        var acceptResponse = await Client.PostAsync($"/api/bookings/{bookingId}/accept", null);
        if (acceptResponse.StatusCode != HttpStatusCode.NoContent)
        {
            var acceptBody = await acceptResponse.Content.ReadAsStringAsync();
            Console.WriteLine("==== ACCEPT RESPONSE START ====");
            Console.WriteLine($"Status: {(int)acceptResponse.StatusCode} {acceptResponse.ReasonPhrase}");
            Console.WriteLine("Body:");
            Console.WriteLine(string.IsNullOrWhiteSpace(acceptBody) ? "<empty>" : acceptBody);
            Console.WriteLine("==== ACCEPT RESPONSE END ====");
        }
        acceptResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse2 = await Client.GetAsync($"/api/bookings/{bookingId}");
        getResponse2.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await getResponse2.Content.ReadFromJsonAsync<BookingDto>();
        updated.Should().NotBeNull();

        Context.ChangeTracker.Clear();
        var dbBookingAfter = await Context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bookingId);
        var dbXminAfter = dbBookingAfter != null ? Context.Entry(dbBookingAfter).Property("xmin").CurrentValue : null;
        Console.WriteLine($"[TEST] Booking after accept (API): {System.Text.Json.JsonSerializer.Serialize(updated)}");
        Console.WriteLine($"[TEST] Booking after accept (DB): {System.Text.Json.JsonSerializer.Serialize(dbBookingAfter)} xmin={dbXminAfter}");

        var dbStatus = dbBookingAfter != null ? dbBookingAfter.Status.ToString() : "<not found>";
        if (updated!.Status != "Accepted")
        {
            throw new Exception($"Booking status after accept (API): {updated.Status}, Booking status after accept (DB): {dbStatus}");
        }

    }

    [Fact]
    public async Task GetAllBookings_ReturnsBookings()
    {
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

        var response = await Client.GetAsync("/api/bookings");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedBookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
        returnedBookings.Should().NotBeNull();
        returnedBookings!.Count.Should().Be(3);
        returnedBookings.Should().AllSatisfy(b => b.OwnerId.Should().Be(owner.Id));
    }
}