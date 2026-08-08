namespace Common.Messaging.Abstractions.Events
{
    public interface ITransactionalEventCollector
    {
        void Add(IEvent eventMessage);
    }
}
