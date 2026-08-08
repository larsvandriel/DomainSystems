namespace Common.Persistence.Transactions.Abstractions
{
    public interface ITransactionManager
    {
        Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}
