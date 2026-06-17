using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain
{
    public sealed class InventoryStock
    {
        public Guid ItemId { get; }
        public Quantity Quantity { get; private set; }

        private InventoryStock(Guid itemId, Quantity quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        public static InventoryStock Create(Guid itemId, Quantity quantity)
        {
            if(itemId == Guid.Empty)
            {
                throw new ArgumentException("ItemId cannot be empty.", nameof(itemId));
            }

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            return new InventoryStock(itemId, quantity);
        }

        public void Increase(Quantity quantity, IUnitConverter unitConverter)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegative(quantity.Value, nameof(quantity));

            if (Quantity.Unit == quantity.Unit)
            {
                Quantity = Quantity.Add(quantity);
                return;
            }

            if (!unitConverter.CanConvert(quantity.Unit, Quantity.Unit))
            {
                throw new InvalidOperationException($"Cannot convert '{quantity.Unit}' to '{Quantity.Unit}'.");
            }

            var convertedQuantity = unitConverter.Convert(quantity, Quantity.Unit);

            Quantity = Quantity.Add(convertedQuantity);
        }

        public void Decrease(Quantity quantity, IUnitConverter unitConverter)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            ArgumentOutOfRangeException.ThrowIfNegative(quantity.Value, nameof(quantity));

            if (Quantity.Unit == quantity.Unit)
            {
                Quantity = Quantity.Subtract(quantity);
                return;
            }

            if (!unitConverter.CanConvert(quantity.Unit, Quantity.Unit))
            {
                throw new InvalidOperationException($"Cannot convert '{quantity.Unit}' to '{Quantity.Unit}'.");
            }

            var convertedQuantity = unitConverter.Convert(quantity, Quantity.Unit);

            Quantity = Quantity.Subtract(convertedQuantity);
        }
    }
}
