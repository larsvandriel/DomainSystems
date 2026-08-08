namespace Common.Messaging.Abstractions.Events
{
    public interface ITransactionalEventBuffer
    {
        IReadOnlyList<IEvent> TakeAll();
        void Clear();
    }
}
