namespace Common.Messaging.Abstractions.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync(IEvent eventMessage, CancellationToken cancellationToken = default);
        
        Task PublishAsync<TEvent>(TEvent eventMessage, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }
}
