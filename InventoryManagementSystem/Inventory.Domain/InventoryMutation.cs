using Inventory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain
{
    public sealed class InventoryMutation
    {
        public Guid ItemId { get; }
        public Quantity Quantity { get; }
        public InventoryMutationType Type { get; }
        public DateTime CreatedAt { get; }

        private InventoryMutation(Guid itemId, Quantity quantity, InventoryMutationType type)
        {
            ItemId = itemId;
            Quantity = quantity;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        public static InventoryMutation CreateIncrease(Guid itemId, Quantity quantity)
        {
            if(itemId == Guid.Empty)
            {
                throw new ArgumentException("ItemId cannot be empty.", nameof(itemId));
            }

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(itemId, quantity, InventoryMutationType.Increase);
        }

        public static InventoryMutation CreateDecrease(Guid itemId, Quantity quantity)
        {
            if (itemId == Guid.Empty)
            {
                throw new ArgumentException("ItemId cannot be empty.", nameof(itemId));
            }

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(itemId, quantity, InventoryMutationType.Decrease);
        }

        public static InventoryMutation CreateAdjustment(Guid itemId, Quantity quantity)
        {
            if (itemId == Guid.Empty)
            {
                throw new ArgumentException("ItemId cannot be empty.", nameof(itemId));
            }

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(itemId, quantity, InventoryMutationType.Adjustment);
        }
    }
}
