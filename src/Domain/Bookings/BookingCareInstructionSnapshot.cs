namespace Domain.Bookings;

public class BookingCareInstructionSnapshot
{
    private BookingCareInstructionSnapshot() { }

    private BookingCareInstructionSnapshot(Guid id, string text)
    {
        Id = id;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid BookingId { get; private set; }
    public string Text { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public static BookingCareInstructionSnapshot Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Instruction cannot be empty.", nameof(text));
        return new BookingCareInstructionSnapshot(Guid.NewGuid(), text.Trim());
    }
}