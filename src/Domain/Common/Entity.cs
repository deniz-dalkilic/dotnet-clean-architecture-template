namespace Template.Domain.Common;

public abstract class Entity<TId>
    where TId : notnull
{
    private readonly List<DomainEvent> _domainEvents = [];

    public TId Id { get; protected init; } = default!;
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
