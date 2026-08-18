using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Stock.AdjustStock
{
    public sealed class AdjustStockCommandValidator : IRequestValidator<AdjustStockCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(AdjustStockCommand request, CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();

            if (request.ItemId == Guid.Empty)
                result.Add(nameof(request.ItemId), "The stock count must have a given itemId.");

            if (string.IsNullOrWhiteSpace(request.ItemName))
                result.Add(nameof(request.ItemName), "The stock count must have a given itemName.");

            if (request.Amount < 0)
                result.Add(nameof(request.Amount), "The stock count must have a non-negative amount.");

            if (string.IsNullOrWhiteSpace(request.Unit))
                result.Add(nameof(request.Unit), "The stock count must have a given unit.");

            return ValueTask.FromResult(result);
        }
    }
}
