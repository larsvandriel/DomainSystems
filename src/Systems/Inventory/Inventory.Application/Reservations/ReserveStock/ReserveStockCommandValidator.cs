using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Reservations.ReserveStock
{
    public sealed class ReserveStockCommandValidator : IRequestValidator<ReserveStockCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(ReserveStockCommand request, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ItemId == Guid.Empty)
                result.Add(nameof(request.ItemId), "ItemId cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                result.Add(nameof(request.Unit), "Unit cannot be empty.");

            if (request.Amount <= 0)
                result.Add(nameof(request.Amount), "Amount must be a positive number.");

            if (request.ExpiresAt is not null && request.ExpiresAt <= DateTimeOffset.UtcNow)
                result.Add(nameof(request.ExpiresAt), "ExpiresAt should not be in the past.");

            return ValueTask.FromResult(result);
        }
    }
}
