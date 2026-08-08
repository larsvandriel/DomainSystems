using Common.Messaging.Abstractions.Events;
using Common.Messaging.Abstractions.PubSub;
using Common.Messaging.Abstractions.Requests;
using Common.Messaging.Sync.Events;
using Common.Messaging.Sync.PubSub;
using Common.Messaging.Sync.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Common.Messaging.Sync.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonMessagingSync(this IServiceCollection services, params Assembly[] handlerAssemblies)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(handlerAssemblies);
            
            services.TryAddScoped<ISyncRequestDispatcher, SyncRequestDispatcher>();
            services.TryAddScoped<ISyncEventDispatcher, SyncEventDispatcher>();
            services.TryAddSingleton<IEventBus, EventBus>();

            services.AddHandlers(handlerAssemblies);

            return services;
        }

        private static IServiceCollection AddHandlers(this IServiceCollection services, Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetTypes()).Where(x => x is { IsAbstract: false, IsInterface: false });

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition() == typeof(ISyncRequestHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(ISyncEventHandler<>)
                        ));

                foreach (var serviceType in interfaces)
                {
                    services.AddScoped(serviceType, type);
                }
            }

            return services;
        }
    }
}
