using Common.Resilience.Backoff;
using Common.Resilience.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Resilience.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonResilience(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            
            services.TryAddSingleton<IRetryDelayCalculator, ExponentialBackoffDelayCalculator>();

            services.TryAddSingleton<IRetryExecutor, RetryExecutor>();

            return services;
        }
    }
}
