namespace Domain.Bookings;

public enum BookingStatus
{
    Requested = 1,
    Accepted = 2,
    Rejected = 3,
    InProgress = 4,
    Completed = 5,
    CancelledByOwner = 6,
    CancelledBySitter = 7,
    Expired = 8
}