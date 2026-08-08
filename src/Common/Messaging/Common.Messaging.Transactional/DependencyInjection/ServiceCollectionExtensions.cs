using Common.Messaging.Abstractions.Events;
using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Common.Messaging.Abstractions.Requests.Commands;
using Common.Messaging.Transactional.Pipelines;
using Common.Persistence.Resilience.DependencyInjection;
using Common.Persistence.Transactions.Abstractions;
using Common.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Common.Messaging.Transactional.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonTransactionalMessaging(
        this IServiceCollection services,
        params Assembly[] handlerAssemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(handlerAssemblies);

        services.AddCommonPersistenceResilience();

        services.TryAddScoped<TransactionalEventBuffer>();

        services.TryAddScoped<ITransactionalEventBuffer>(provider =>
            provider.GetRequiredService<TransactionalEventBuffer>());

        services.TryAddScoped<ITransactionalEventCollector>(provider =>
            provider.GetRequiredService<TransactionalEventBuffer>());

        services.TryAddEnumerable(
            ServiceDescriptor.Scoped<ITransactionParticipant, PublishEventsAfterCommitParticipant>());

        services.AddResilientTransactionBehaviors(handlerAssemblies);

        return services;
    }

    private static IServiceCollection AddResilientTransactionBehaviors(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        var handlerInterfaces = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(type => type.GetInterfaces())
            .Where(type =>
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .Distinct();

        foreach (var handlerInterface in handlerInterfaces)
        {
            var genericArguments = handlerInterface.GetGenericArguments();
            var requestType = genericArguments[0];
            var resultType = genericArguments[1];

            if (!ImplementsTransactionalCommand(requestType, resultType))
                continue;

            Type? behaviorType = null;

            if (resultType == typeof(Result))
            {
                behaviorType = typeof(ResilientTransactionBehavior<>)
                    .MakeGenericType(requestType);
            }
            else if (
                resultType.IsGenericType &&
                resultType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = resultType.GetGenericArguments()[0];

                behaviorType = typeof(ResilientTransactionBehavior<,>)
                    .MakeGenericType(requestType, valueType);
            }

            if (behaviorType is null)
                continue;

            var serviceType = typeof(IRequestPipelineBehavior<,>)
                .MakeGenericType(requestType, resultType);

            services.TryAddEnumerable(
                ServiceDescriptor.Scoped(serviceType, behaviorType));
        }

        return services;
    }

    private static bool ImplementsTransactionalCommand(
        Type requestType,
        Type resultType)
    {
        return requestType
            .GetInterfaces()
            .Any(type =>
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(ITransactionalCommand<>) &&
                type.GetGenericArguments()[0] == resultType);
    }
}
