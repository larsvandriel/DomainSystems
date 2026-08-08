using Common.Persistence.Transactions.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Persistence.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonPersistenceEntityFrameworkCore<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);
            
            services.TryAddScoped<IUnitOfWork, EfUnitOfWork<TDbContext>>();
            services.TryAddScoped<ITransactionManager, EfTransactionManager<TDbContext>>();

            return services;
        }
    }
}
