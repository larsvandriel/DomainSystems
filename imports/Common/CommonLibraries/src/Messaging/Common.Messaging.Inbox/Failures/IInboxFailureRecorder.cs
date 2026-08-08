using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Integration.Contracts;
using Common.Results.Problems;

namespace Common.Messaging.Inbox.Failures
{
    public interface IInboxFailureRecorder
    {
        Task RecordAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Exception exception,
            CancellationToken cancellationToken = default);

        Task RecordAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Problem problem,
            CancellationToken cancellationToken = default);
    }
}
