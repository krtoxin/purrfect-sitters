namespace Domain.Bookings;

public class BookingStatusHistory
{
    private BookingStatusHistory() { }

    private BookingStatusHistory(Guid id, BookingStatus status, DateTime changedAtUtc)
    {
        Id = id;
        Status = status;
        ChangedAtUtc = changedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }

    public static BookingStatusHistory Create(BookingStatus status) =>
        new(Guid.NewGuid(), status, DateTime.UtcNow);
}