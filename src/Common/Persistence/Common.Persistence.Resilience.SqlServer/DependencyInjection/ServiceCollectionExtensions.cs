using Common.Persistence.Resilience.Classification;
using Common.Persistence.Resilience.DependencyInjection;
using Common.Persistence.Resilience.Execution;
using Common.Persistence.Resilience.SqlServer.Classification;
using Common.Persistence.Resilience.SqlServer.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Persistence.Resilience.SqlServer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonPersistenceResilienceSqlServer(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddCommonPersistenceResilience();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITransactionRetryExceptionClassifier, SqlServerTransactionRetryExceptionClassifier>());

            services.TryAddSingleton<IResilientReadExecutor, SqlServerResilientReadExecutor>();

            return services;
        }
    }
}
