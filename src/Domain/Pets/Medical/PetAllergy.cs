namespace Domain.Pets.Medical;

public enum AllergySeverity
{
    Mild = 1,
    Moderate = 2,
    Severe = 3,
    Critical = 4
}

public class PetAllergy
{
    private PetAllergy() { }

    private PetAllergy(Guid id, string name, AllergySeverity severity, string? notes)
    {
        Id = id;
        Name = name;
        Severity = severity;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PetId { get; private set; }
    public string Name { get; private set; } = default!;
    public AllergySeverity Severity { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PetAllergy Create(Guid id, string name, AllergySeverity severity, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Allergy name required.");
        return new PetAllergy(id, name.Trim(), severity, string.IsNullOrWhiteSpace(notes) ? null : notes.Trim());
    }
}