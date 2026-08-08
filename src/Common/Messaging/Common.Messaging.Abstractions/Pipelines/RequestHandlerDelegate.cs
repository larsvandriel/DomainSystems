namespace Common.Messaging.Abstractions.Pipelines
{
    public delegate Task<TResult> RequestHandlerDelegate<TResult>(CancellationToken cancellationToken = default);
}
