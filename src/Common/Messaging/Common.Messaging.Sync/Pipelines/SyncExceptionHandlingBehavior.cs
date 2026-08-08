using Common.Messaging.Abstractions.Pipelines;
using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Common.Results.Problems;
using Microsoft.Extensions.Logging;

namespace Common.Messaging.Sync.Pipelines
{
    public sealed class SyncExceptionHandlingBehavior<TRequest>(
        ILogger<SyncExceptionHandlingBehavior<TRequest>> logger) : ISyncRequestPipelineBehavior<TRequest, Result> where TRequest : IRequest<Result>
    {
        public Result Handle(TRequest request, SyncRequestHandlerDelegate<Result> continuation)
        {
            try
            {
                return continuation();
            }
            catch (Exception exception)
            {
                SyncExceptionHandlingLog.LogPipelineFailure(logger, exception, typeof(TRequest).Name);

                return Result.Failure(ProblemFactory.Unexpected());
            }
        }
    }

    public sealed class SyncExceptionHandlingBehavior<TRequest, TValue>(
        ILogger<SyncExceptionHandlingBehavior<TRequest, TValue>> logger) : ISyncRequestPipelineBehavior<TRequest, Result<TValue>> where TRequest : IRequest<Result<TValue>>
    {
        public Result<TValue> Handle(TRequest request, SyncRequestHandlerDelegate<Result<TValue>> continuation)
        {
            try
            {
                return continuation();
            }
            catch (Exception exception)
            {
                SyncExceptionHandlingLog.LogPipelineFailure(logger, exception, typeof(TRequest).Name);
                return Result.Failure<TValue>(ProblemFactory.Unexpected());
            }
        }
    }

    internal static partial class SyncExceptionHandlingLog
    {
        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Error,
            Message = "An unexpected error occurred while handling the request of type {RequestType}.")]
        internal static partial void LogPipelineFailure(ILogger logger, Exception exception, string requestType);
    }
}
