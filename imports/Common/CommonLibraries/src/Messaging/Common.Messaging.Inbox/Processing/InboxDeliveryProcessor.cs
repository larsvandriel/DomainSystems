using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Inbox.Exceptions;
using Common.Messaging.Integration.Contracts;
using Common.Results;
using Common.Results.Problems;

namespace Common.Messaging.Inbox.Processing
{
    public sealed class InboxDeliveryProcessor(IInboxProcessor inboxProcessor)
    {
        public async Task HandleAsync<THandler>(
            IntegrationEventEnvelope envelope,
            string consumer,
            IInboxBrokerDelivery delivery,
            Func<THandler, IntegrationEventEnvelope, CancellationToken, Task<Result>> handler,
            CancellationToken cancellationToken = default)
            where THandler : notnull
        {
            Result result;

            try
            {
                result = await inboxProcessor.ProcessAsync(envelope, consumer, delivery.Context, handler, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                await SettleExceptionAsync(delivery, exception, cancellationToken);

                return;
            }

            if (result.IsSuccess)
            {
                await delivery.CompleteAsync(cancellationToken);
                return;
            }


            await SettleProblemAsync(delivery, result.Problem!, cancellationToken);
        }

        private static Task SettleExceptionAsync(IInboxBrokerDelivery delivery, Exception exception, CancellationToken cancellationToken)
        {
            if (delivery.Context.IsFinalAttempt)
            {
                return delivery.DeadLetterAsync(reason: "inbox.processing_failed", description: exception.Message, cancellationToken);
            }

            return delivery.RetryAsync(exception, cancellationToken);
        }

        private static Task SettleProblemAsync(IInboxBrokerDelivery delivery, Problem problem, CancellationToken cancellationToken)
        {
            if (delivery.Context.IsFinalAttempt)
            {
                return delivery.DeadLetterAsync(reason: problem.Code, description: problem.Detail ?? problem.Title, cancellationToken);
            }

            return delivery.RetryAsync(new InboxMessageRejectedException(problem.Code, problem.Detail ?? problem.Title), cancellationToken);
        }
    }
}
