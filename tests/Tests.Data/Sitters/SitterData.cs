using Domain.Sitters;
using Domain.Users;
using Domain.ValueObjects;

namespace Tests.Data.Sitters;

public static class SitterData
{
    public static Sitter CreateSitter(
        Guid id = default,
        string firstName = "Jane",
        string lastName = "Smith",
        string email = "test.sitter@example.com",
        string phone = "+1234567890",
        string bio = "Experienced pet sitter",
        decimal hourlyRate = 25.00m,
        Address? address = null,
        bool isAvailable = true)
    {
        address ??= new Address("123 Test St", null, "Test City", "Test State", "12345", "Test Country");

        return new Sitter(
            id == default ? Guid.NewGuid() : id,
            firstName,
            lastName,
            email,
            Email.Create(email),
            phone,
            address,
            bio,
            Money.Create(hourlyRate, "USD"),
            isAvailable
        );
    }

    public static IEnumerable<Sitter> CreateSitters(int count = 3, bool isAvailable = true)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateSitter(
                firstName: $"Sitter{i}",
                lastName: $"Test{i}",
                email: $"sitter{i}@example.com",
                hourlyRate: 20.00m + i * 5,
                isAvailable: isAvailable
            ));
    }

    public static SitterProfile CreateSitterProfile(
        Guid id = default,
        Guid userId = default,
        string bio = "Experienced pet sitter",
        decimal baseRate = 25.00m,
        SitterServiceType services = SitterServiceType.DayVisit | SitterServiceType.Walking)
    {
        if (id == default) id = Guid.NewGuid();
        if (userId == default) userId = Guid.NewGuid();
        
        var profile = SitterProfile.Create(
            id,
            userId,
            bio,
            Money.Create(baseRate, "USD"),
            services
        );

        profile.AddAvailability(
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(1).AddHours(8)
        );

        return profile;
    }

    public static IEnumerable<SitterProfile> CreateSitterProfiles(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateSitterProfile(
                baseRate: 20.00m + i * 5
            ));
    }
}