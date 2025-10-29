namespace Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void Raise(IDomainEvent @event) => _domainEvents.Add(@event);
    public void ClearEvents() => _domainEvents.Clear();
}

public interface IDomainEvent { DateTime OccurredOnUtc { get; } }