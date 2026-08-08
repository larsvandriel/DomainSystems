namespace Common.Messaging.Outbox.Contracts
{
    public interface IOutboxWriter
    {
        void Add(OutboxMessage message);

        void AddRange(IReadOnlyCollection<OutboxMessage> messages);
    }
}
