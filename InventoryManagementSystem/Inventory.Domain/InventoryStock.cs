using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Inventory.Domain
{
    public sealed class InventoryStock
    {
        public InventoryItem Item { get; }
        public Quantity Quantity { get; private set; }

        private readonly List<InventoryMutation> _mutations = [];

        public IReadOnlyCollection<InventoryMutation> Mutations => _mutations;

        private InventoryStock(InventoryItem item, Quantity quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public static InventoryStock Create(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryStock(item, quantity);
        }

        public static InventoryStock Restore(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryStock(item, quantity);
        }

        public void Increase(Quantity quantity, IUnitConverter unitConverter)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            ArgumentNullException.ThrowIfNull(unitConverter, nameof(unitConverter));
            ArgumentOutOfRangeException.ThrowIfNegative(quantity.Value, nameof(quantity.Value));

            var quantityToAdd = Quantity.Unit == quantity.Unit ? quantity : unitConverter.Convert(quantity, Quantity.Unit);

            Quantity = Quantity.Add(quantityToAdd);

            _mutations.Add(InventoryMutation.CreateIncrease(Item, quantity));
        }

        public void Decrease(Quantity quantity, IUnitConverter unitConverter)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
            ArgumentNullException.ThrowIfNull(unitConverter, nameof(unitConverter));
            ArgumentOutOfRangeException.ThrowIfNegative(quantity.Value, nameof(quantity.Value));

            var quantityToDecrease = Quantity.Unit == quantity.Unit ? quantity : unitConverter.Convert(quantity, Quantity.Unit);

            Quantity = Quantity.Subtract(quantityToDecrease);

            _mutations.Add(InventoryMutation.CreateDecrease(Item, quantity));
        }

        public void Adjust(Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));
                        
            Quantity = quantity;

            _mutations.Add(InventoryMutation.CreateAdjustment(Item, quantity));
        }

        public IReadOnlyList<InventoryMutation> DequeueMutations()
        {
            var mutations = _mutations.ToList();
            _mutations.Clear();
            return mutations;
        }
    }
}
