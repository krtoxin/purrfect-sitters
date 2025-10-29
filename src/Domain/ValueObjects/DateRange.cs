namespace Domain.ValueObjects;

public sealed record DateRange(DateTime StartUtc, DateTime EndUtc)
{
    public static DateRange Create(DateTime startUtc, DateTime endUtc)
    {
        if (startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Dates must be UTC.");
        if (endUtc <= startUtc)
            throw new ArgumentException("End must be after start.");
        return new DateRange(startUtc, endUtc);
    }

    public bool Overlaps(DateRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;
}