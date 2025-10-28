using Domain.Bookings;
using Domain.Sitters;
using Domain.ValueObjects;

namespace Tests.Data.Bookings;

public static class BookingData
{
    public static Booking CreateBooking(
        Guid id = default,
        Guid petId = default,
        Guid ownerId = default,
        Guid sitterProfileId = default,
        DateTime? startTime = null,
        DateTime? endTime = null,
        decimal? totalAmount = null,
        SitterServiceType serviceType = SitterServiceType.DayVisit,
        IEnumerable<string>? careInstructions = null)
    {
        if (id == default) id = Guid.NewGuid();
        if (petId == default) petId = Guid.NewGuid();
        if (ownerId == default) ownerId = Guid.NewGuid();
        if (sitterProfileId == default) sitterProfileId = Guid.NewGuid();
        
        startTime ??= DateTime.UtcNow.AddDays(1);
        endTime ??= startTime.Value.AddHours(2);
        careInstructions ??= new[] { "Take good care of the pets" };
        
        var price = BookingPrice.Create(totalAmount ?? 50.00m, 10m, "USD"); // 10% service fee
        
        return Booking.Create(
            id,
            petId,
            ownerId,
            sitterProfileId,
            startTime.Value,
            endTime.Value,
            price,
            serviceType,
            careInstructions
        );
    }

    public static IEnumerable<Booking> CreateBookings(
        int count = 3,
        Guid? ownerId = null,
        Guid? sitterProfileId = null,
        Guid? petId = null)
    {
        var baseDate = DateTime.UtcNow.AddDays(1);
        
        return Enumerable.Range(1, count)
            .Select(i => CreateBooking(
                ownerId: ownerId ?? Guid.NewGuid(),
                sitterProfileId: sitterProfileId ?? Guid.NewGuid(),
                petId: petId ?? Guid.NewGuid(),
                startTime: baseDate.AddDays(i),
                endTime: baseDate.AddDays(i).AddHours(2),
                careInstructions: new[] { $"Special instructions for booking {i}" },
                totalAmount: 50.00m * i
            ));
    }
}