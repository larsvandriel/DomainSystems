namespace Common.Resilience.Execution
{
    public sealed record RetryAttempt(int FailedAttempt, int MaximumAttempts, Exception Exception, TimeSpan Delay);
}
