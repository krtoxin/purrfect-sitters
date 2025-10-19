namespace Domain.Pets.Medical;

public class MedicalEntry
{
    private MedicalEntry() { }

    private MedicalEntry(Guid id, string title, string? details)
    {
        Id = id;
        Title = title;
        Details = details;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PetId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Details { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static MedicalEntry Create(Guid id, string title, string? details)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title required.", nameof(title));
        return new MedicalEntry(id, title.Trim(), string.IsNullOrWhiteSpace(details) ? null : details.Trim());
    }
}