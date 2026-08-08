using Common.Messaging.Abstractions.Events;
using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.PubSub;
using Common.Messaging.Abstractions.Requests;
using Common.Messaging.Abstractions.Validation;
using Common.Messaging.Async.Events;
using Common.Messaging.Async.Pipelines;
using Common.Messaging.Async.PubSub;
using Common.Messaging.Async.Requests;
using Common.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Common.Messaging.Async.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonMessagingAsync(this IServiceCollection services, params Assembly[] handlerAssemblies)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(handlerAssemblies);
            
            services.TryAddScoped<IRequestDispatcher, RequestDispatcher>();
            services.TryAddScoped<IEventDispatcher, EventDispatcher>();
            services.TryAddSingleton<IAsyncEventBus, AsyncEventBus>();

            services.AddHandlers(handlerAssemblies);
            services.AddValidators(handlerAssemblies);
            services.AddRequestPipelineBehaviors(handlerAssemblies);

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
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                    ));

                foreach (var serviceType in interfaces)
                {
                    services.AddScoped(serviceType, type);
                }
            }

            return services;
        }

        private static IServiceCollection AddValidators(this IServiceCollection services, Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => type is { IsAbstract: false, IsInterface: false });

            foreach (var implementationType in types)
            {
                var validationInterfaces = implementationType.GetInterfaces()
                    .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestValidator<>));

                foreach (var serviceType in validationInterfaces)
                    services.AddScoped(serviceType, implementationType);
            }

            return services;
        }

        private static IServiceCollection AddRequestPipelineBehaviors(this IServiceCollection services, Assembly[] assemblies)
        {
            var handlerInterfaces = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type is { IsAbstract: false, IsInterface: false })
                .SelectMany(type => type.GetInterfaces())
                .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Distinct()
                .ToArray();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var genericArguments = handlerInterface.GetGenericArguments();
                var requestType = genericArguments[0];
                var resultType = genericArguments[1];

                var serviceType = typeof(IRequestPipelineBehavior<,>).MakeGenericType(requestType, resultType);

                if (resultType == typeof(Result))
                {
                    RegisterBehavior(services, serviceType, typeof(ExceptionHandlingBehavior<>).MakeGenericType(requestType));

                    RegisterBehavior(services, serviceType, typeof(ValidationBehavior<>).MakeGenericType(requestType));

                    continue;
                }

                if (!resultType.IsGenericType || resultType.GetGenericTypeDefinition() != typeof(Result<>))
                    continue;

                var valueType = resultType.GetGenericArguments()[0];

                RegisterBehavior(services, serviceType, typeof(ExceptionHandlingBehavior<,>).MakeGenericType(requestType, valueType));

                RegisterBehavior(services, serviceType, typeof(ValidationBehavior<,>).MakeGenericType(requestType, valueType));
            }

            return services;
        }

        private static void RegisterBehavior(IServiceCollection services, Type serviceType, Type implementationType)
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
        }
    }
}
