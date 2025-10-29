using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Owners;

public class OwnerProfile : AggregateRoot
{
    private readonly List<EmergencyContact> _emergencyContacts = new();
    
    private OwnerProfile() { }

    private OwnerProfile(Guid id, Guid userId, string? defaultNotes, Address? address, string? timezone)
    {
        Id = id;
        UserId = userId;
        DefaultCareNotes = defaultNotes;
        Address = address;
        PreferredTimezone = timezone;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public Guid UserId { get; private set; }
    public string? DefaultCareNotes { get; private set; }
    public Address? Address { get; private set; }
    public string? PreferredTimezone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<EmergencyContact> EmergencyContacts => _emergencyContacts;

    public static OwnerProfile Create(Guid id, Guid userId, string? defaultNotes = null, Address? address = null, string? timezone = null)
        => new(id, userId, string.IsNullOrWhiteSpace(defaultNotes) ? null : defaultNotes.Trim(), address, timezone);

    public EmergencyContact AddEmergencyContact(string name, string phone)
    {
        var contact = EmergencyContact.Create(Guid.NewGuid(), name, phone);
        _emergencyContacts.Add(contact);
        UpdatedAt = DateTime.UtcNow;
        return contact;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}