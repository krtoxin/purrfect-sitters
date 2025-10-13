using Application.Common.Interfaces;
using Domain.Bookings;
using Domain.Pets;
using Domain.Sitters;
using Domain.Users;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestDataController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IPetRepository _pets;
    private readonly ISitterProfileRepository _sitters;
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;

    public TestDataController(
        IUserRepository users,
        IPetRepository pets,
        ISitterProfileRepository sitters,
        IBookingRepository bookings,
        IUnitOfWork uow)
    {
        _users = users;
        _pets = pets;
        _sitters = sitters;
        _bookings = bookings;
        _uow = uow;
    }

    [HttpPost("seed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedTestData(CancellationToken ct)
    {
        // Create test users
        var owner = Domain.Users.User.Register(Guid.NewGuid(), Email.Create("owner@test.com"), "John Owner", UserRole.Owner);
        var sitter = Domain.Users.User.Register(Guid.NewGuid(), Email.Create("sitter@test.com"), "Jane Sitter", UserRole.Sitter);
        
        await _users.AddAsync(owner, ct);
        await _users.AddAsync(sitter, ct);

        // Create test pet
        var pet = Pet.Create(Guid.NewGuid(), owner.Id, "Buddy", PetType.Dog, "Golden Retriever", "Very friendly dog");
        pet.AddInstruction("Feed twice daily");
        pet.AddInstruction("Walk for 30 minutes");
        
        await _pets.AddAsync(pet, ct);

        // Create test sitter profile
        var sitterProfile = SitterProfile.Create(
            Guid.NewGuid(), 
            sitter.Id, 
            "Experienced pet sitter with 5 years of experience", 
            Money.Create(50m, "USD"), 
            SitterServiceType.DayVisit);
        
        await _sitters.AddAsync(sitterProfile, ct);

        // Create test booking
        var price = BookingPrice.Create(100m, 10m, "USD");
        var booking = Booking.Create(
            Guid.NewGuid(),
            pet.Id,
            owner.Id,
            sitterProfile.Id,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            price,
            SitterServiceType.DayVisit,
            new[] { "Feed twice daily", "Walk for 30 minutes" });

        await _bookings.AddAsync(booking, ct);

        await _uow.SaveChangesAsync(ct);

        return Ok(new
        {
            OwnerId = owner.Id,
            SitterId = sitter.Id,
            PetId = pet.Id,
            SitterProfileId = sitterProfile.Id,
            BookingId = booking.Id,
            Message = "Test data created successfully!"
        });
    }
}
