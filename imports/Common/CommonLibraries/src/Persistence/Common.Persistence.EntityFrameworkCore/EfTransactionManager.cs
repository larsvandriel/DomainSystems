using Common.Persistence.Transactions.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EntityFrameworkCore
{
    public sealed class EfTransactionManager<TDbContext>(TDbContext dbContext) : ITransactionManager where TDbContext : DbContext
    {
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }
    }
}
