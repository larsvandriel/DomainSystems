using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Persistence.EntityFrameworkCore.SqlServer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDuplicateKeyConflictDetection(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IDbUpdateConcurrencyConflictDetector, SqlServerDuplicateKeyConflictDetector>());

            return services;
        }
    }
}
