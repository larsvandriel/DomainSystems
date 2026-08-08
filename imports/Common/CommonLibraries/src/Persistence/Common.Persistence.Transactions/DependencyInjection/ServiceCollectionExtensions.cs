using Common.Persistence.Transactions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Persistence.Transactions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonPersistenceTransactions(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddScoped<ITransactionExecutor, TransactionExecutor>();

            return services;
        }
    }
}
