using Domain.Common;
using Domain.Pets.Medical;

namespace Domain.Pets;

public class Pet : AggregateRoot
{
    private readonly List<CareInstruction> _instructions = new();
    private readonly List<MedicalEntry> _medicalEntries = new();
    private readonly List<PetAllergy> _allergies = new();
    private readonly List<PetVaccination> _vaccinations = new();
    private readonly List<PetMedication> _medications = new();

    private Pet() { }

    private Pet(
        Guid id,
        Guid ownerId,
        string name,
        PetType type,
        string? breed,
        string? notes)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
        Type = type;
        Breed = breed;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid OwnerId { get; private set; }
    public string Name { get; private set; } = default!;
    public PetType Type { get; private set; }
    public string? Breed { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<CareInstruction> Instructions => _instructions;
    public IReadOnlyCollection<MedicalEntry> MedicalEntries => _medicalEntries;
    public IReadOnlyCollection<PetAllergy> Allergies => _allergies;
    public IReadOnlyCollection<PetVaccination> Vaccinations => _vaccinations;
    public IReadOnlyCollection<PetMedication> Medications => _medications;

    public static Pet Create(
        Guid id,
        Guid ownerId,
        string name,
        PetType type,
        string? breed = null,
        string? notes = null)
        => new(id, ownerId, name, type, breed, notes);

    public void AddInstruction(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Instruction cannot be empty.", nameof(text));

        _instructions.Add(CareInstruction.Create(Guid.NewGuid(), text));
    }

    public MedicalEntry AddMedicalEntry(string title, string? details)
    {
        var entry = MedicalEntry.Create(Guid.NewGuid(), title, details);
        _medicalEntries.Add(entry);
        return entry;
    }

    public PetAllergy AddAllergy(string name, AllergySeverity severity, string? notes = null)
    {
        if (_allergies.Any(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Allergy '{name}' already exists.");

        var allergy = PetAllergy.Create(Guid.NewGuid(), name, severity, notes);
        _allergies.Add(allergy);
        return allergy;
    }

    public PetVaccination AddVaccination(string vaccineName, DateTime administeredOnUtc, DateTime? expiresOnUtc = null)
    {
        if (administeredOnUtc > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException("Administered date cannot be in the future.", nameof(administeredOnUtc));

        var vacc = PetVaccination.Create(Guid.NewGuid(), vaccineName, administeredOnUtc, expiresOnUtc);
        _vaccinations.Add(vacc);
        return vacc;
    }

    public PetMedication PrescribeMedication(string name, string dosage, string schedule, DateTime startUtc, DateTime? endUtc = null)
    {
        if (startUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Start must be UTC.");
        if (endUtc.HasValue && endUtc.Value <= startUtc) throw new ArgumentException("End must be after start.");

        var med = PetMedication.Create(Guid.NewGuid(), name, dosage, schedule, startUtc, endUtc);
        _medications.Add(med);
        return med;
    }

    public void MarkMedicationEnded(Guid medicationId, DateTime endedAtUtc)
    {
        var med = _medications.FirstOrDefault(m => m.Id == medicationId)
                  ?? throw new InvalidOperationException("Medication not found.");
        med.MarkEnded(endedAtUtc);
    }
}

public class CareInstruction
{
    private CareInstruction() { }

    private CareInstruction(Guid id, string text)
    {
        Id = id;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid PetId { get; private set; }
    public string Text { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public static CareInstruction Create(Guid id, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Instruction text cannot be empty.", nameof(text));
        return new CareInstruction(id, text.Trim());
    }
}