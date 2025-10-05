using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Sitters;

public class SitterProfile : AggregateRoot
{
    private readonly List<AvailabilitySlot> _availability = new();
    private SitterProfile() { }

    private SitterProfile(Guid id, Guid userId, string bio, Money? baseRate, SitterServiceType services)
    {
        Id = id;
        UserId = userId;
        Bio = bio;
        BaseRate = baseRate;
        ServicesOffered = services;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
        AverageRating = 0m;
    }

    public Guid UserId { get; private set; }
    public string Bio { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public decimal AverageRating { get; private set; }
    public Money? BaseRate { get; private set; }
    public SitterServiceType ServicesOffered { get; private set; }
    public int CompletedBookings { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<AvailabilitySlot> Availability => _availability;

    public static SitterProfile Create(Guid id, Guid userId, string bio, Money? baseRate, SitterServiceType services)
        => new(id, userId, string.IsNullOrWhiteSpace(bio) ? "" : bio.Trim(), baseRate, services);

    public void AddAvailability(DateTime startUtc, DateTime endUtc)
    {
        _availability.Add(AvailabilitySlot.Create(Guid.NewGuid(), startUtc, endUtc));
    }

    public void IncrementCompletedBookings()
    {
        CompletedBookings++;
    }

    public void UpdateRating(decimal newValue)
    {
        if (newValue < 0 || newValue > 5) throw new ArgumentException("Rating must be 0–5.");
        AverageRating = (AverageRating + newValue) / 2m;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}