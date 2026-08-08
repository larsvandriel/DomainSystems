using Common.Persistence.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.EntityFrameworkCore.SqlServer
{
    public sealed class SqlServerDuplicateKeyConflictDetector : IDbUpdateConcurrencyConflictDetector
    {
        private const int DuplicateIndexKey = 2601;
        private const int DuplicateConstraintKey = 2627;

        public bool IsConcurrencyConflict(DbUpdateException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            
            var sqlException = FindSqlException(exception);

            return sqlException?.Errors.Cast<SqlError>().Any(error => error.Number is DuplicateIndexKey or DuplicateConstraintKey) == true;
        }

        private static SqlException? FindSqlException(Exception exception)
        {
            for (Exception? current = exception; current is not null; current = current.InnerException)
            {
                if (current is SqlException sqlException)
                    return sqlException;
            }

            return null;
        }
    }
}
