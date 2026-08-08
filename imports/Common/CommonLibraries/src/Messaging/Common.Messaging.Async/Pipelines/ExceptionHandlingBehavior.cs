using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Common.Results.Problems;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Async.Pipelines
{
    public sealed class ExceptionHandlingBehavior<TRequest>(
        ILogger<ExceptionHandlingBehavior<TRequest>> logger) : IRequestPipelineBehavior<TRequest, Result> where TRequest : IRequest<Result>
    {
        public async Task<Result> HandleAsync(TRequest request, RequestHandlerDelegate<Result> continuation, CancellationToken cancellationToken = default)
        {
            try
            {
                return await continuation(cancellationToken);
            }
            catch(OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch(Exception exception)
            {
                ExceptionHandlingLog.LogPipelineFailure(logger, exception, typeof(TRequest).Name);

                return Result.Failure(ProblemFactory.Unexpected());
            }
        }
    }

    public sealed class ExceptionHandlingBehavior<TRequest, TValue>(
        ILogger<ExceptionHandlingBehavior<TRequest, TValue>> logger) : IRequestPipelineBehavior<TRequest, Result<TValue>> where TRequest : IRequest<Result<TValue>>
    {
        public async Task<Result<TValue>> HandleAsync(TRequest request, RequestHandlerDelegate<Result<TValue>> continuation, CancellationToken cancellationToken = default)
        {
            try
            {
                return await continuation(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                ExceptionHandlingLog.LogPipelineFailure(logger, exception, typeof(TRequest).Name);

                return Result.Failure<TValue>(ProblemFactory.Unexpected());
            }
        }
    }

    internal static partial class ExceptionHandlingLog
    {
        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Error,
            Message = "An unexpected error occurred while handling the request of type {RequestType}.")]
        internal static partial void LogPipelineFailure(ILogger logger, Exception exception, string requestType);
    }
}
