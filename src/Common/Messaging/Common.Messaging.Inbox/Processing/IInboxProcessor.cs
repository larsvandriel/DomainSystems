using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Integration.Contracts;
using Common.Results;

namespace Common.Messaging.Inbox.Processing
{
    public interface IInboxProcessor
    {
        Task<Result> ProcessAsync<THandler>(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Func<THandler, IntegrationEventEnvelope, CancellationToken, Task<Result>> handler,
            CancellationToken cancellationToken = default) where THandler : notnull;
    }
}
