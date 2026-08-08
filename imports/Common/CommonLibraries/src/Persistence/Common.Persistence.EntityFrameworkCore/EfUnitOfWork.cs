using Common.Persistence.Concurrency;
using Common.Persistence.Transactions.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EntityFrameworkCore
{
    public sealed class EfUnitOfWork<TDbContext>(
        TDbContext dbContext,
        IEnumerable<IDbUpdateConcurrencyConflictDetector> conflictDetectors) : IUnitOfWork where TDbContext : DbContext
    {
        private readonly DbContext _dbContext = dbContext;

        private readonly IReadOnlyCollection<IDbUpdateConcurrencyConflictDetector> _conflictDetectors = [.. conflictDetectors];

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException exception)
            {
                throw new ConcurrencyConflictException("The persisted data was changed by another operation.", exception);
            }
            catch(DbUpdateException exception) when(IsConcurrencyConflict(exception))
            {
                throw new ConcurrencyConflictException("A record with the same key was created by another operation.", exception);
            }
        }

        private bool IsConcurrencyConflict(DbUpdateException exception)
        {
            return _conflictDetectors.Any(detector => detector.IsConcurrencyConflict(exception));
        }

        
    }
}
