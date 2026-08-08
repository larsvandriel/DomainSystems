using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Integration.Contracts;
using Common.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Inbox.Processing
{
    public sealed class InboxProcessingAttempt(IInboxStore inboxStore, IServiceProvider serviceProvider, TimeProvider timeProvider)
    {
        public async Task<Result> ExecuteAsync<THandler>(
            IntegrationEventEnvelope envelope,
            string consumer,
            Func<THandler, IntegrationEventEnvelope, CancellationToken, Task<Result>> handler,
            CancellationToken cancellationToken)
            where THandler : notnull
        {
            var existingMessage = await inboxStore.FindAsync(envelope.MessageId, consumer, cancellationToken);

            if (existingMessage is not null)
                return Result.Success();

            var receivedAtUtc = timeProvider.GetUtcNow();

            var resolvedHandler = serviceProvider.GetRequiredService<THandler>();

            var result = await handler(resolvedHandler, envelope, cancellationToken);

            if (result.IsFailure)
                return result;

            var inboxMessage = InboxMessage.CreateProcessed(envelope.MessageId, consumer, envelope.Identifier, receivedAtUtc, timeProvider.GetUtcNow());

            inboxStore.Add(inboxMessage);

            return result;
        }
    }
}
