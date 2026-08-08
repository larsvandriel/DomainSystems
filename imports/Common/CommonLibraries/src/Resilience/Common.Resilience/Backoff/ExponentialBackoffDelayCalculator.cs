namespace Common.Resilience.Backoff
{
    public sealed class ExponentialBackoffDelayCalculator : IRetryDelayCalculator
    {
        public TimeSpan CalculateDelay(int failedAttempt, RetryOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(failedAttempt);

            if(options.InitialDelay == TimeSpan.Zero)
                return TimeSpan.Zero;

            var multiplier = Math.Pow(2, failedAttempt - 1);

            var milliseconds = Math.Min(options.InitialDelay.TotalMilliseconds * multiplier, options.MaximumDelay.TotalMilliseconds);

            if(options.UseJitter)
                milliseconds *= Random.Shared.NextDouble() * 0.5 + 0.75;

            milliseconds = Math.Min(milliseconds, options.MaximumDelay.TotalMilliseconds);

            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
