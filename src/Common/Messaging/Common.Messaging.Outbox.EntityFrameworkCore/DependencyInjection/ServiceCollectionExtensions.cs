using Common.Messaging.Outbox.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Messaging.Outbox.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonMessagingOutboxEntityFrameworkCore<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddScoped<IOutboxWriter, EfOutboxWriter<TDbContext>>();

            services.TryAddScoped<IOutboxStore, EfOutboxStore<TDbContext>>();

            return services;
        }
    }
}
