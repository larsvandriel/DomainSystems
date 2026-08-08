using Microsoft.Extensions.Options;

namespace Common.Messaging.Outbox.Configuration
{
    public sealed class OutboxOptionsValidator : IValidateOptions<OutboxOptions>
    {
        public ValidateOptionsResult Validate(string? name, OutboxOptions options)
        {
            var errors = new List<string>();

            if (options.BatchSize < 1)
                errors.Add($"{nameof(options.BatchSize)} must be at least 1.");

            if (options.MaximumAttempts < 1)
                errors.Add($"{nameof(options.MaximumAttempts)} must be at least 1.");

            if (options.InitialRetryDelay < TimeSpan.Zero)
                errors.Add($"{nameof(options.InitialRetryDelay)} must be zero or positive.");

            if (options.MaximumRetryDelay < TimeSpan.Zero)
                errors.Add($"{nameof(options.MaximumRetryDelay)} must be zero or positive.");

            if (options.MaximumRetryDelay < options.InitialRetryDelay)
                errors.Add($"{nameof(options.MaximumRetryDelay)} must be greater than or equal to {nameof(options.InitialRetryDelay)}.");

            return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
        }
    }
}
