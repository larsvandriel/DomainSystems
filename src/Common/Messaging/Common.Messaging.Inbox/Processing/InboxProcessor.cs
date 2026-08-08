using Common.Messaging.Inbox.Contracts;
using Common.Messaging.Inbox.Failures;
using Common.Messaging.Integration.Contracts;
using Common.Persistence.Resilience.Execution;
using Common.Results;
using Common.Results.Problems;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Inbox.Processing
{
    public sealed partial class InboxProcessor(
        IResilientTransactionExecutor transactionExecutor,
        IInboxFailureRecorder failureRecorder,
        ILogger<InboxProcessor> logger) : IInboxProcessor
    {
        public async Task<Result> ProcessAsync<THandler>(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Func<THandler, IntegrationEventEnvelope, CancellationToken, Task<Result>> handler,
            CancellationToken cancellationToken = default)
            where THandler: notnull
        {
            ArgumentNullException.ThrowIfNull(envelope);
            ArgumentException.ThrowIfNullOrWhiteSpace(consumer);
            ArgumentNullException.ThrowIfNull(delivery);
            ArgumentNullException.ThrowIfNull(handler);

            try
            {
                var result = await transactionExecutor.ExecuteAsync<InboxProcessingAttempt>((attempt, attemptCancellationToken) =>
                    attempt.ExecuteAsync(envelope, consumer, handler, attemptCancellationToken),
                    cancellationToken);

                if (result.IsFailure)
                {
                    var problem = result.Problem!;
                    var disposition = GetDisposition(delivery);

                    LogMessageRejected(logger, envelope.MessageId, consumer, problem.Code, delivery.DeliveryAttempt, disposition);

                    await RecordFailureSafelyAsync(envelope, consumer, delivery, problem);
                }

                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                LogProcessingFailure(logger, exception, envelope.MessageId, consumer, delivery.DeliveryAttempt);

                await RecordFailureSafelyAsync(envelope, consumer, delivery, exception);

                throw;
            }
        }

        private static InboxFailureDisposition GetDisposition(InboxDeliveryContext delivery)
        {
            return delivery.IsFinalAttempt ? InboxFailureDisposition.DeadLetterRequested : InboxFailureDisposition.RetryRequested;
        }

        private async Task RecordFailureSafelyAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Problem problem)
        {
            try
            {
                await failureRecorder.RecordAsync(envelope, consumer, delivery, problem, CancellationToken.None);
            }
            catch(Exception failureRecordingException)
            {
                LogFailureRecordingFailure(
                    logger,
                    failureRecordingException,
                    envelope.MessageId,
                    consumer,
                    nameof(Problem),
                    problem.Detail ?? problem.Title);
            }
        }

        private async Task RecordFailureSafelyAsync(
            IntegrationEventEnvelope envelope,
            string consumer,
            InboxDeliveryContext delivery,
            Exception originalException)
        {
            try
            {
                await failureRecorder.RecordAsync(envelope, consumer, delivery, originalException, CancellationToken.None);
            }
            catch (Exception failureRecordingException)
            {
                LogFailureRecordingFailure(
                    logger,
                    failureRecordingException,
                    envelope.MessageId,
                    consumer,
                    originalException.GetType().FullName ?? originalException.GetType().Name,
                    originalException.Message);
            }
        }

        [LoggerMessage(
    EventId = 1101,
    Level = LogLevel.Warning,
    Message =
        "Inbox message {MessageId} for consumer {Consumer} was rejected " +
        "with error code {ErrorCode} on delivery attempt {DeliveryAttempt}. " +
        "Disposition {Disposition} was requested.")]
        private static partial void LogMessageRejected(
    ILogger logger,
    Guid messageId,
    string consumer,
    string errorCode,
    int deliveryAttempt,
    InboxFailureDisposition disposition);

        [LoggerMessage(
            EventId = 1102,
            Level = LogLevel.Error,
            Message =
                "Processing inbox message {MessageId} for consumer {Consumer} " +
                "failed on delivery attempt {DeliveryAttempt}.")]
        private static partial void LogProcessingFailure(
            ILogger logger,
            Exception exception,
            Guid messageId,
            string consumer,
            int deliveryAttempt);

        [LoggerMessage(
            EventId = 1103,
            Level = LogLevel.Error,
            Message =
                "Persisting failure details for inbox message {MessageId} " +
                "and consumer {Consumer} failed. " +
                "Original failure: {OriginalErrorType}: {OriginalErrorMessage}")]
        private static partial void LogFailureRecordingFailure(
            ILogger logger,
            Exception exception,
            Guid messageId,
            string consumer,
            string originalErrorType,
            string originalErrorMessage);
    }
}
