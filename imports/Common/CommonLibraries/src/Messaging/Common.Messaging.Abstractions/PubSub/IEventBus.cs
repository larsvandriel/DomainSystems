namespace Common.Messaging.Abstractions.PubSub
{
    public interface IEventBus
    {
        IDisposable Subscribe<TEvent>(Action<TEvent> handler);

        void Publish<TEvent>(TEvent eventMessage);
    }
}
