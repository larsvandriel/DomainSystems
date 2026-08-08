using Common.Resilience.Backoff;

namespace Common.Resilience.Execution
{
    public sealed class RetryExecutor(IRetryDelayCalculator delayCalculator) : IRetryExecutor
    {
        public async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            Func<Exception, bool> shouldRetry,
            RetryOptions options,
            Func<RetryAttempt, CancellationToken, Task>? onRetry = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action);
            ArgumentNullException.ThrowIfNull(shouldRetry);
            ArgumentNullException.ThrowIfNull(options);

            Validate(options);

            for (var attempt = 1; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await action(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception) when (attempt < options.MaxAttempts && shouldRetry(exception))
                {
                    var delay = delayCalculator.CalculateDelay(attempt, options);

                    if (onRetry is not null)
                        await onRetry(new RetryAttempt(attempt, options.MaxAttempts, exception, delay), cancellationToken);
                    
                    if(delay > TimeSpan.Zero)
                        await Task.Delay(delay, cancellationToken);
                }
            }
        }

        private static void Validate(RetryOptions options)
        {
            if(options.MaxAttempts < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(options),
                    options.MaxAttempts,
                    $"{nameof(RetryOptions.MaxAttempts)} must be at least 1.");

            if (options.InitialDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(
                    nameof(options),
                    options.InitialDelay,
                    $"{nameof(RetryOptions.InitialDelay)} cannot be nagative.");

            if( options.MaximumDelay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(
                    nameof(options),
                    options.MaximumDelay,
                    $"{nameof(RetryOptions.MaximumDelay)} cannot be nagative.");

            if (options.MaximumDelay < options.InitialDelay)
                throw new ArgumentException(
                    $"{nameof(RetryOptions.MaximumDelay)} must be greater than or equal to {nameof(RetryOptions.InitialDelay)}.",
                    nameof(options));
        }
    }
}
