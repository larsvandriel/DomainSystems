namespace Common.Messaging.Abstractions.Events
{
    public interface ISyncEventDispatcher
    {
        void Publish<TEvent>(TEvent eventMessage) where TEvent : IEvent;
    }
}
