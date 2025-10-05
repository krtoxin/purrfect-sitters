namespace Domain.Owners;

public class EmergencyContact
{
    private EmergencyContact() { }

    private EmergencyContact(Guid id, string name, string phone)
    {
        Id = id;
        Name = name;
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    public static EmergencyContact Create(Guid id, string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Contact name required.");
        if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Phone required.");
        return new EmergencyContact(id, name.Trim(), phone.Trim());
    }
}