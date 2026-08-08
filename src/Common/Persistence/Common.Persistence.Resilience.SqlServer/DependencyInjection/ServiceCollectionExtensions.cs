using Common.Persistence.Resilience.Classification;
using Common.Persistence.Resilience.SqlServer.Classification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Persistence.Resilience.SqlServer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonPersistenceResilienceSqlServer(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITransactionRetryExceptionClassifier, SqlServerTransactionRetryExceptionClassifier>());

            return services;
        }
    }
}
