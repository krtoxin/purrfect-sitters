namespace Domain.Pets.Medical;

public class PetMedication
{
    private PetMedication() { }

    private PetMedication(Guid id, string name, string dosage, string schedule, DateTime startUtc, DateTime? endUtc)
    {
        Id = id;
        Name = name;
        Dosage = dosage;
        Schedule = schedule;
        StartUtc = startUtc;
        EndUtc = endUtc;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Dosage { get; private set; } = default!;
    public string Schedule { get; private set; } = default!;
    public DateTime StartUtc { get; private set; }
    public DateTime? EndUtc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    internal static PetMedication Create(Guid id, string name, string dosage, string schedule, DateTime startUtc, DateTime? endUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Medication name required.");
        if (string.IsNullOrWhiteSpace(dosage)) throw new ArgumentException("Dosage required.");
        if (string.IsNullOrWhiteSpace(schedule)) throw new ArgumentException("Schedule required.");
        return new PetMedication(id, name.Trim(), dosage.Trim(), schedule.Trim(), startUtc, endUtc);
    }

    internal void MarkEnded(DateTime endedAtUtc)
    {
        if (endedAtUtc <= StartUtc) throw new ArgumentException("End must be after start.");
        EndUtc = endedAtUtc;
    }
}