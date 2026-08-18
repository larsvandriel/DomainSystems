using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Stock.IncreaseStock
{
    public sealed class IncreaseStockCommandValidator : IRequestValidator<IncreaseStockCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(IncreaseStockCommand request, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();
            if (request.ItemId == Guid.Empty)
            {
                result.Add(nameof(request.ItemId), "ItemId must not be empty.");
            }
            if (string.IsNullOrWhiteSpace(request.ItemName))
            {
                result.Add(nameof(request.ItemName), "ItemName must not be empty.");
            }
            if (request.Amount <= 0)
            {
                result.Add(nameof(request.Amount), "Amount must be greater than zero.");
            }
            if (string.IsNullOrWhiteSpace(request.Unit))
            {
                result.Add(nameof(request.Unit), "Unit must not be empty.");
            }
            return ValueTask.FromResult(result);
        }
    }
}
