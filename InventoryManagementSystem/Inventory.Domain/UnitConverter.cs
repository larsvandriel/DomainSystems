using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain
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

        public bool CanConvert(string fromUnit, string toUnit)
        {
            return fromUnit == toUnit || _factors.ContainsKey((fromUnit, toUnit));
        }

        public Quantity Convert(Quantity quantity, string targetUnit)
        {
            if(quantity.Unit == targetUnit)
                return quantity;
            
            if(!_factors.TryGetValue((quantity.Unit, targetUnit), out var factor))
            {
                throw new InvalidOperationException($"Cannot convert from '{quantity.Unit}' to '{targetUnit}'.");
            }

            return Quantity.Create(quantity.Value * factor, targetUnit);
        }
    }
}
