namespace Common.Messaging.Abstractions.Events
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent eventMessage, CancellationToken cancellationToken = default);
    }
}
