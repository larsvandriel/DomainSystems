using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Integration.Contracts;
using Common.Persistence.Transactions.Execution;
using Common.Results;
using Common.Results.Problems;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.Inbox.Failures
{
    public sealed class InboxFailureRecorder(IServiceScopeFactory scopeFactory, TimeProvider timeProvider) : IInboxFailureRecorder
    {
        public Task RecordAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Exception exception,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return RecordInternalAsync(
                envelope,
                consumer,
                delivery,
                errorType: exception.GetType().FullName ?? exception.GetType().Name,
                errorMessage: exception.Message,
                errorCode: null,
                cancellationToken);
        }

        public Task RecordAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Problem problem,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(problem);

            return RecordInternalAsync(
                envelope,
                consumer,
                delivery,
                errorType: nameof(Problem),
                errorMessage: problem.Detail ?? problem.Title,
                errorCode: problem.Code,
                cancellationToken);
        }

        private async Task RecordInternalAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            string errorType,
            string errorMessage,
            string? errorCode,
            CancellationToken cancellationToken)
        {
            await using var scope = scopeFactory.CreateAsyncScope();

            var store = scope.ServiceProvider.GetRequiredService<IInboxFailureStore>();

            var transactionExecutor = scope.ServiceProvider.GetRequiredService<ITransactionExecutor>();

            var disposition = delivery.IsFinalAttempt ? InboxFailureDisposition.DeadLetterRequested : InboxFailureDisposition.RetryRequested;

            await transactionExecutor.ExecuteAsync(_ =>
            {
                var failure = InboxFailure.Create(
                    envelope.MessageId,
                    consumer,
                    envelope.Identifier,
                    delivery.DeliveryAttempt,
                    timeProvider.GetUtcNow(),
                    disposition,
                    errorType,
                    errorMessage,
                    errorCode,
                    delivery.TraceId ?? envelope.TraceId);

                store.Add(failure);

                return Task.FromResult(Result.Success());
            }, cancellationToken);
        }
    }
}
