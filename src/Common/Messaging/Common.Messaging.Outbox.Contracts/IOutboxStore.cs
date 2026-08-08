namespace Common.Messaging.Outbox.Contracts
{
    public interface IOutboxStore
    {
        Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int maximumCount, DateTimeOffset nowUtc, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
