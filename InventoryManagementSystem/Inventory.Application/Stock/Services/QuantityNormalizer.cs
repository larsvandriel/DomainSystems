using Common.Results;
using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

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

            return Result<Quantity>.Failure(ProblemDetailsFactory.BusinessRule(
                type: "error:UnitConversionNotSupported.",
                detail: $"Cannot convert from {quantity.Unit} to {targetUnit}."));

        }
    }
}
