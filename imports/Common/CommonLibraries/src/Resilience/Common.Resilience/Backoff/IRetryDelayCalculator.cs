namespace Common.Resilience.Backoff
{
    public interface IRetryDelayCalculator
    {
        TimeSpan CalculateDelay(int failedAttempt, RetryOptions options);
    }
}
