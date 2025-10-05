namespace Domain.Pets.Medical;

public class PetVaccination
{
    private PetVaccination() { }

    private PetVaccination(Guid id, string vaccineName, DateTime administeredOnUtc, DateTime? expiresOnUtc)
    {
        Id = id;
        VaccineName = vaccineName;
        AdministeredOnUtc = administeredOnUtc;
        ExpiresOnUtc = expiresOnUtc;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string VaccineName { get; private set; } = default!;
    public DateTime AdministeredOnUtc { get; private set; }
    public DateTime? ExpiresOnUtc { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PetVaccination Create(Guid id, string vaccineName, DateTime administeredOnUtc, DateTime? expiresOnUtc)
    {
        if (string.IsNullOrWhiteSpace(vaccineName))
            throw new ArgumentException("Vaccine name required.", nameof(vaccineName));
        return new PetVaccination(id, vaccineName.Trim(), administeredOnUtc, expiresOnUtc);
    }
}