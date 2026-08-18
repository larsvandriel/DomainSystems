using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.Services
{
    public sealed class QuantityCalculator(QuantityNormalizer quantityNormalizer)
    {
        public Result<Quantity> Sum(IEnumerable<Quantity> quantities, string targetUnit)
        {
            ArgumentNullException.ThrowIfNull(quantities);
            ArgumentException.ThrowIfNullOrWhiteSpace(targetUnit);

            decimal total = 0;

            foreach (var quantity in quantities)
            {
                var normalizedResult = quantityNormalizer.NormalizeTo(quantity, targetUnit);

                if (normalizedResult.IsFailure)
                    return normalizedResult;

                total += normalizedResult.Value.Value;
            }

            return Quantity.Create(total, targetUnit);
        }

        public Result<Quantity> Add(Quantity left, Quantity right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            var normalizedRightResult = quantityNormalizer.NormalizeTo(right, left.Unit);
            if (normalizedRightResult.IsFailure)
                return normalizedRightResult;

            return left.Add(normalizedRightResult.Value);
        }

        public Result<Quantity> Subtract(Quantity left, Quantity right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);

            var normalizedRightResult = quantityNormalizer.NormalizeTo(right, left.Unit);
            if (normalizedRightResult.IsFailure)
                return normalizedRightResult;

            return left.Subtract(normalizedRightResult.Value);
        }
    }
}
