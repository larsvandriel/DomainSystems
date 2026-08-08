using Common.Persistence.Transactions.Abstractions;
using Common.Persistence.Transactions.Exceptions;
using Common.Results;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Common.Persistence.Transactions.Execution
{
    public sealed partial class TransactionExecutor(
        ITransactionManager transactionManager,
        IUnitOfWork unitOfWork,
        IEnumerable<ITransactionParticipant> participants,
        ILogger<TransactionExecutor> logger)
        : ITransactionExecutor
    {
        private readonly ITransactionManager _transactionManager = transactionManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IReadOnlyCollection<ITransactionParticipant> _participants = [.. participants];
        private readonly ILogger<TransactionExecutor> _logger = logger;

        public Task<Result> ExecuteAsync(Func<CancellationToken, Task<Result>> action, CancellationToken cancellationToken = default)
        {
            return ExecuteInternalAsync(action, result => result.IsFailure, cancellationToken);
        }

        public Task<Result<T>> ExecuteAsync<T>(Func<CancellationToken, Task<Result<T>>> action, CancellationToken cancellationToken = default)
        {
            return ExecuteInternalAsync(action, result => result.IsFailure, cancellationToken);
        }

        private async Task<TResult> ExecuteInternalAsync<TResult>(
            Func<CancellationToken, Task<TResult>> action,
            Func<TResult, bool> shouldRollback,
            CancellationToken cancellationToken)
        {
            await using var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);

            TResult result;

            try
            {
                result = await action(cancellationToken);

                if (shouldRollback(result))
                {
                    await RollbackSafelyAsync(transaction);
                    await NotifyAbortedSafelyAsync();
                    return result;
                }

                foreach (var participant in _participants)
                {
                    await participant.PrepareAsync(cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception originalException)
            {
                await RollbackSafelyAsync(transaction, originalException);
                await NotifyAbortedSafelyAsync(originalException);
                throw;
            }

            foreach (var participant in _participants)
            {
                try
                {
                    await participant.CommittedAsync(CancellationToken.None);
                }
                catch (Exception exception)
                {
                    throw new PostCommitException(
                        $"Transaction participant '{participant.GetType().FullName}' failed after the transaction was committed.",
                        exception);
                }
            }

            return result;
        }

        private async Task RollbackSafelyAsync(ITransaction transaction, Exception? originalException = null)
        {
            try
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }
            catch (Exception rollbackException)
            {
                if(originalException is null)
                {
                    LogRollbackFailure(_logger, rollbackException);
                    return;
                }
                
                LogOriginalExceptionAfterRollbackFailure(
                    _logger,
                    originalException,
                    originalException.GetType().FullName ?? originalException.GetType().Name,
                    originalException.Message);
            }
        }

        private async Task NotifyAbortedSafelyAsync(Exception? originalException = null)
        {
            foreach (var participant in _participants)
            {
                try
                {
                    await participant.AbortedAsync(CancellationToken.None);
                }
                catch (Exception participantException)
                {
                    var participantType = participant.GetType().FullName ?? participant.GetType().Name;

                    if(originalException is null)
                    {
                        LogParticipantAbortedFailure(_logger, participantException, participantType);
                        continue;
                    }

                    LogParticipantAbortedFailureAfterOriginalException(
                        _logger,
                        participantException,
                        participantType,
                        originalException.GetType().FullName ?? originalException.GetType().Name,
                        originalException.Message);
                }
            }
        }

        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Error,
            Message = "Rolling back the transaction failed.")]
        private static partial void LogRollbackFailure(ILogger logger, Exception exception);

        [LoggerMessage(
            EventId = 1006,
            Level = LogLevel.Error,
            Message = """
                Rolling back the transaction failed after an earlier exception.
                The original exception will be rethrown.
                Original exception: {OriginalExceptionType}: {OriginalExceptionMessage}
            """)]
        private static partial void LogOriginalExceptionAfterRollbackFailure(
            ILogger logger,
            Exception exception,
            string originalExceptionType,
            string originalExceptionMessage);

        [LoggerMessage(
            EventId = 1007,
            Level = LogLevel.Error,
            Message = "Transaction participant {ParticipantType} failed while handling an aborted transaction.")]
        private static partial void LogParticipantAbortedFailure(
            ILogger logger,
            Exception exception,
            string participantType);

        [LoggerMessage(
            EventId = 1008,
            Level = LogLevel.Error,
            Message = """
                Transaction participant {ParticipantType} failed while handling an aborted transaction.
                Original exception: {OriginalExceptionType}: {OriginalExceptionMessage}
            """)]
        private static partial void LogParticipantAbortedFailureAfterOriginalException(
            ILogger logger,
            Exception exception,
            string participantType,
            string originalExceptionType,
            string originalExceptionMessage);
    }
}
