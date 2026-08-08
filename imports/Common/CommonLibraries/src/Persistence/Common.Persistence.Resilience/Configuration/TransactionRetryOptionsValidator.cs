using Microsoft.Extensions.Options;

namespace Common.Persistence.Resilience.Configuration
{
    public sealed class TransactionRetryOptionsValidator : IValidateOptions<TransactionRetryOptions>
    {
        public ValidateOptionsResult Validate(string? name, TransactionRetryOptions options)
        {
            var errors = new List<string>();

            if (options.MaxAttempts < 1)
                errors.Add($"{nameof(options.MaxAttempts)} must be at least 1.");

            if (options.InitialDelay < TimeSpan.Zero)
                errors.Add($"{nameof(options.InitialDelay)} cannot be negative.");

            if (options.MaximumDelay < TimeSpan.Zero)
                errors.Add($"{nameof(options.MaximumDelay)} cannot be negative.");

            if (options.MaximumDelay < options.InitialDelay)
                errors.Add($"{nameof(options.MaximumDelay)} must be greater than or equal to {nameof(options.InitialDelay)}.");

            return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
        }
    }
}
