using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EntityFrameworkCore
{
    public interface IDbUpdateConcurrencyConflictDetector
    {
        bool IsConcurrencyConflict(DbUpdateException exception);
    }
}
