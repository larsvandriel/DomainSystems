using Common.Persistence.Resilience.Classification;
using Common.Persistence.Resilience.Configuration;
using Common.Persistence.Resilience.Execution;
using Common.Persistence.Transactions.DependencyInjection;
using Common.Resilience.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Common.Persistence.Resilience.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonPersistenceResilience(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddCommonResilience();
            services.AddCommonPersistenceTransactions();

            services.AddOptions<TransactionRetryOptions>().BindConfiguration(TransactionRetryOptions.SectionName).ValidateOnStart();

            services.TryAddSingleton<IResilientTransactionExecutor, ResilientTransactionExecutor>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITransactionRetryExceptionClassifier, ConcurrencyConflictExceptionClassifier>());

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TransactionRetryOptions>, TransactionRetryOptionsValidator>());

            return services;
        }
    }
}
