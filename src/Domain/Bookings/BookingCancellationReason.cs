namespace Domain.Bookings;

public enum BookingCancellationReason
{
    Unspecified = 0,
    OwnerChangedMind = 1,
    SitterUnavailable = 2,
    PetHealthIssue = 3,
    Emergency = 4,
    PricingDisagreement = 5,
    SchedulingConflict = 6
}