using Inventory.Domain.Models;

namespace Inventory.Application.Stock.Services
{
    public class UnitConverter : IUnitConverter
    {
        private readonly Dictionary<(string From, string To), decimal> _factors = new()
        {
            { ("kg", "g"), 1000m },
            { ("g", "kg"), 0.001m },
            { ("l", "ml"), 1000m },
            { ("ml", "l"), 0.001m },
            { ("l", "cl"), 100m },
            { ("cl", "l"), 0.01m },
            { ("cl", "ml"), 10m },
            { ("ml", "cl"), 0.1m }
            // Add more conversion factors as needed
        };

        public bool CanConvert(string sourceUnit, string targetUnit)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceUnit);
            ArgumentException.ThrowIfNullOrWhiteSpace(targetUnit);

            sourceUnit = sourceUnit.Trim();
            targetUnit = targetUnit.Trim();

            return string.Equals(sourceUnit, targetUnit, StringComparison.OrdinalIgnoreCase) || _factors.ContainsKey((sourceUnit, targetUnit));
        }

        public Quantity Convert(Quantity sourceQuantity, string targetUnit)
        {
            if(TryConvert(sourceQuantity, targetUnit, out var convertedQuantity))
                return convertedQuantity!;

            throw new InvalidOperationException($"Cannot convert from {sourceQuantity.Unit} to {targetUnit}");
        }

        public bool TryConvert(Quantity sourceQuantity, string targetUnit, out Quantity? convertedQuantity)
        {
            ArgumentNullException.ThrowIfNull(sourceQuantity);
            ArgumentException.ThrowIfNullOrWhiteSpace(targetUnit);

            var sourceUnit = sourceQuantity.Unit.Trim().ToLowerInvariant();
            targetUnit = targetUnit.Trim().ToLowerInvariant();
            
            if(string.Equals(sourceUnit, targetUnit, StringComparison.OrdinalIgnoreCase))
            {
                convertedQuantity = sourceQuantity.Unit == targetUnit ? sourceQuantity : Quantity.Create(sourceQuantity.Value, targetUnit);
                return true;
            }

            if(!_factors.TryGetValue((sourceUnit, targetUnit), out var factor))
            {
                convertedQuantity = null;
                return false;
            }

            convertedQuantity = Quantity.Create(sourceQuantity.Value * factor, targetUnit);
            return true;
        }
    }
}
