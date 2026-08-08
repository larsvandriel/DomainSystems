namespace Common.Messaging.Abstractions.Events.Domain
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
    }
}
