using Common.Messaging.Inbox.Failures;
using Common.Messaging.Inbox.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Messaging.Inbox.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonMessagingInbox(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAddSingleton(TimeProvider.System);

            services.TryAddSingleton<IInboxProcessor, InboxProcessor>();

            services.TryAddSingleton<IInboxFailureRecorder, InboxFailureRecorder>();

            services.TryAddScoped<InboxProcessingAttempt>();

            services.TryAddSingleton<InboxDeliveryProcessor>();

            return services;
        }
    }
}
