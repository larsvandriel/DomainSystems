using Common.Messaging.Inbox.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Messaging.Inbox.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonMessagingInboxEntityFrameworkCore<TDbContext>(
            this IServiceCollection services) where TDbContext : DbContext
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddScoped<IInboxStore, EfInboxStore<TDbContext>>();

            services.TryAddScoped<IInboxFailureStore, EfInboxFailureStore<TDbContext>>();

            return services;
        }
    }
}
