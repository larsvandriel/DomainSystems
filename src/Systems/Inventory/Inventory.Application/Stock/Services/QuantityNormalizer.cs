using Common.Results;
using Common.Results.Problems;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.Services
{
    public sealed class QuantityNormalizer(IUnitConverter unitConverter)
    {
        private readonly IUnitConverter _unitConverter = unitConverter;

        public Result<Quantity> NormalizeTo(Quantity quantity, string targetUnit)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            ArgumentException.ThrowIfNullOrWhiteSpace(targetUnit, nameof(targetUnit));

            if (quantity.Unit.Equals(targetUnit, StringComparison.OrdinalIgnoreCase))
                return quantity;

            if (_unitConverter.TryConvert(quantity, targetUnit, out var convertedQuantity))
                return convertedQuantity!;

            return Result.Failure<Quantity>(ProblemFactory.BusinessRule(
                code: "error:UnitConversionNotSupported.",
                detail: $"Cannot convert from {quantity.Unit} to {targetUnit}."));

        }
    }
}
