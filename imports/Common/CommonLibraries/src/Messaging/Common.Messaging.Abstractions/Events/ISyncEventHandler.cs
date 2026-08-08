namespace Common.Messaging.Abstractions.Events
{
    public interface ISyncEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent eventMessage);
    }
}
