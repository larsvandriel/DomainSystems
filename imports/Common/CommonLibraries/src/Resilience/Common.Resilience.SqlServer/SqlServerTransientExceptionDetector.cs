using Microsoft.Data.SqlClient;

namespace Common.Resilience.SqlServer
{
    public static class SqlServerTransientExceptionDetector
    {
        private static readonly HashSet<int> TransientErrorNumbers =
        [
            -2,     // Timeout
            1205,   // Deadlock
            4060,   // Cannot open database requested by the login. The login failed.
            10928,  // Resource ID %d. The %s limit for the database is %d and has been reached.
            10929,  // Resource ID %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. However, the server is currently too busy to support requests greater than %d for this database.
            40197,  // The service has encountered an error processing your request. Please try again.
            40501,  // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
            40613   // Database XXXX on server YYYY is not currently available. Please retry the connection later.
        ];

        public static bool IsTransient(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            for (Exception? current = exception; current is not null; current = current.InnerException)
            {
                if (current is TimeoutException)
                    return true;

                if (current is SqlException sqlException && sqlException.Errors.Cast<SqlError>().Any(error => TransientErrorNumbers.Contains(error.Number)))
                    return true;
            }

            return false;
        }
    }
}
