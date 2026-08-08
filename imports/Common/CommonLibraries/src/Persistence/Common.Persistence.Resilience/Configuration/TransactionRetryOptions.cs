using Common.Resilience;

namespace Common.Persistence.Resilience.Configuration
{
    public sealed class TransactionRetryOptions
    {
        public const string SectionName = "Persistence:TransactionRetry";

        public int MaxAttempts { get; init; } = 3;

        public TimeSpan InitialDelay { get; init; } = TimeSpan.FromMilliseconds(20);

        public TimeSpan MaximumDelay { get; init; } = TimeSpan.FromMilliseconds(250);

        public bool UseJitter { get; init; } = true;

        internal RetryOptions ToRetryOptions() => new()
        {
            MaxAttempts = MaxAttempts,
            InitialDelay = InitialDelay,
            MaximumDelay = MaximumDelay,
            UseJitter = UseJitter
        };
    }
}
