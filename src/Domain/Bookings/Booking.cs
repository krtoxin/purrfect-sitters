using Domain.Common;
using Domain.Bookings.Events;
using Domain.Sitters;

namespace Domain.Bookings;

public class Booking : AggregateRoot
{
    private readonly List<BookingStatusHistory> _statusHistory = new();
    private readonly List<BookingCareInstructionSnapshot> _careInstructionSnapshots = new();

    private Booking() { }

    private Booking(
        Guid id,
        Guid petId,
        Guid ownerId,
        Guid sitterProfileId,
        DateTime startUtc,
        DateTime endUtc,
        BookingPrice price,
        SitterServiceType serviceType,
        IEnumerable<string> careInstructionTexts)
    {
        if (startUtc.Kind != DateTimeKind.Utc || endUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Start and End must be UTC.");
        if (endUtc <= startUtc)
            throw new ArgumentException("End must be after Start.");

        Id = id;
        PetId = petId;
        OwnerId = ownerId;
        SitterProfileId = sitterProfileId;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Status = BookingStatus.Requested;
        Price = price;
        ServiceType = serviceType;
        CreatedAt = DateTime.UtcNow;
        Version = 0;

        foreach (var text in careInstructionTexts.Distinct().Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            _careInstructionSnapshots.Add(BookingCareInstructionSnapshot.Create(text));
        }

        AddStatusHistory(Status);
        Raise(new BookingRequestedEvent(Id));
    }

    public Guid PetId { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid SitterProfileId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public BookingStatus Status { get; private set; }
    public BookingPrice Price { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public BookingCancellationReason? CancellationReason { get; private set; }
    public SitterServiceType ServiceType { get; private set; }
    public bool IsReviewed { get; private set; }
    public long Version { get; private set; } // optimistic concurrency

    public IReadOnlyCollection<BookingStatusHistory> StatusHistory => _statusHistory;
    public IReadOnlyCollection<BookingCareInstructionSnapshot> CareInstructionSnapshots => _careInstructionSnapshots;

    public static Booking Create(
        Guid id,
        Guid petId,
        Guid ownerId,
        Guid sitterProfileId,
        DateTime startUtc,
        DateTime endUtc,
        BookingPrice price,
        SitterServiceType serviceType,
        IEnumerable<string> careInstructionTexts)
        => new(id, petId, ownerId, sitterProfileId, startUtc, endUtc, price, serviceType, careInstructionTexts);

    public void Accept()
    {
        EnsureStatus(BookingStatus.Requested);
        Transition(BookingStatus.Accepted);
        Raise(new BookingAcceptedEvent(Id));
    }

    public void Reject()
    {
        EnsureStatus(BookingStatus.Requested);
        Transition(BookingStatus.Rejected);
    }

    public void Start()
    {
        EnsureStatus(BookingStatus.Accepted);
        Transition(BookingStatus.InProgress);
    }

    public void Complete()
    {
        EnsureStatus(BookingStatus.InProgress);
        Transition(BookingStatus.Completed);
        CompletedAtUtc = DateTime.UtcNow;
        Raise(new BookingCompletedEvent(Id));
    }

    public void CancelByOwner(BookingCancellationReason reason)
    {
        if (Status is BookingStatus.Completed or BookingStatus.CancelledByOwner or BookingStatus.CancelledBySitter)
            throw new InvalidOperationException("Cannot cancel in current status.");
        Transition(BookingStatus.CancelledByOwner);
        CancelledAtUtc = DateTime.UtcNow;
        CancellationReason = reason;
        Raise(new BookingCancelledEvent(Id, Status, reason));
    }

    public void CancelBySitter(BookingCancellationReason reason)
    {
        if (Status is BookingStatus.Completed or BookingStatus.CancelledByOwner or BookingStatus.CancelledBySitter)
            throw new InvalidOperationException("Cannot cancel in current status.");
        Transition(BookingStatus.CancelledBySitter);
        CancelledAtUtc = DateTime.UtcNow;
        CancellationReason = reason;
        Raise(new BookingCancelledEvent(Id, Status, reason));
    }

    public void MarkReviewed()
    {
        if (Status != BookingStatus.Completed)
            throw new InvalidOperationException("Booking must be completed before marking reviewed.");
        IsReviewed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Requote(decimal newBaseAmount, decimal? newServiceFeePercent = null)
    {
        if (Status != BookingStatus.Requested)
            throw new InvalidOperationException("Only requested bookings can be requoted.");
        Price = Price.Recalculate(newBaseAmount, newServiceFeePercent);
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureStatus(BookingStatus expected)
    {
        if (Status != expected)
            throw new InvalidOperationException($"Expected status {expected} but current status is {Status}.");
    }

    private void Transition(BookingStatus newStatus)
    {
        if (Status == newStatus) return;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        AddStatusHistory(Status);
        Version++; 
    }

    private void AddStatusHistory(BookingStatus status)
        => _statusHistory.Add(BookingStatusHistory.Create(status));
}