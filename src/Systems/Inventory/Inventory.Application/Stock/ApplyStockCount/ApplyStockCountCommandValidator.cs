using Common.Messaging.Abstractions.Validation;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed class ApplyStockCountCommandValidator : IRequestValidator<ApplyStockCountCommand>
    {
        public ValueTask<ValidationResult> ValidateAsync(
            ApplyStockCountCommand request,
            CancellationToken cancellationToken = default)
        {
            var result = new ValidationResult();
            if (request.Lines is null || request.Lines.Count == 0)
            {
                result.Add(nameof(request.Lines), "No stock count lines provided.");
                return ValueTask.FromResult(result);
            }

            if (request.Lines.Any(line => line.ItemId == Guid.Empty))
                result.Add(nameof(request.Lines), "All stock count lines must have a given itemId.");

            if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.ItemName)))
                result.Add(nameof(request.Lines), "All stock count lines must have a given itemName.");

            if (request.Lines.Any(line => line.CountedAmount < 0))
                result.Add(nameof(request.Lines), "All stock count should have non-negative amounts.");

            if (request.Lines.Any(line => string.IsNullOrWhiteSpace(line.Unit)))
                result.Add(nameof(request.Lines), "All stock count lines must have a given unit.");

            ValidateDuplicateItemIds(request.Lines, result);
            return ValueTask.FromResult(result);
        }

        private static void ValidateDuplicateItemIds(IReadOnlyCollection<StockCountLine> lines, ValidationResult result)
        {
            foreach (var message in lines
                .GroupBy(line => line.ItemId)
                .Where(group => group.Count() > 1)
                .Select(group => $"ItemId '{group.Key}' occurs {group.Count()} times."))
            {
                result.Add(nameof(ApplyStockCountCommand.Lines), message);
            }
        }
    }
}
