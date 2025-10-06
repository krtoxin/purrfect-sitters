namespace Domain.Sitters;

public class AvailabilitySlot
{
    private AvailabilitySlot() { }

    private AvailabilitySlot(Guid id, DateTime startUtc, DateTime endUtc)
    {
        if (startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Dates must be UTC.");
        if (endUtc <= startUtc)
            throw new ArgumentException("End must be after start.");

        Id = id;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Status = AvailabilityStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public AvailabilityStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static AvailabilitySlot Create(Guid id, DateTime startUtc, DateTime endUtc)
        => new(id, startUtc, endUtc);

    public void MarkBooked()
    {
        if (Status != AvailabilityStatus.Open) throw new InvalidOperationException("Slot not open.");
        Status = AvailabilityStatus.Booked;
    }

    public void MarkUnavailable()
    {
        if (Status == AvailabilityStatus.Unavailable) return;
        Status = AvailabilityStatus.Unavailable;
    }
}

public enum AvailabilityStatus
{
    Open = 1,
    Booked = 2,
    Unavailable = 3
}