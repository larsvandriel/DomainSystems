namespace Common.Resilience
{
    public sealed class RetryOptions
    {
        public static RetryOptions Default => new();

        public int MaxAttempts { get; init; } = 3;

        public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(250);

        public TimeSpan MaximumDelay { get; init; } = TimeSpan.FromSeconds(10);

        public bool UseJitter { get; init; } = true;
    }
}
