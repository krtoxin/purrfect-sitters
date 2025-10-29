using Domain.Common;

namespace Domain.Bookings.Events;

public record BookingRequestedEvent(Guid BookingId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public record BookingAcceptedEvent(Guid BookingId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public record BookingCancelledEvent(Guid BookingId, BookingStatus Status, BookingCancellationReason Reason) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}

public record BookingCompletedEvent(Guid BookingId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}