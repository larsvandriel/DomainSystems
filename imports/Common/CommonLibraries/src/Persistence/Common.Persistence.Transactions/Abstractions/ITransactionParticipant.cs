namespace Common.Persistence.Transactions.Abstractions
{
    public interface ITransactionParticipant
    {
        Task PrepareAsync(CancellationToken cancellationToken = default);

        Task CommittedAsync(CancellationToken cancellationToken = default);

        Task AbortedAsync(CancellationToken cancellationToken = default);
    }
}
