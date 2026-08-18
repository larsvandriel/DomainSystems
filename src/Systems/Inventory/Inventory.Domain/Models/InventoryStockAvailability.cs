namespace Inventory.Domain.Models
{
    public sealed class InventoryStockAvailability
    {
        public InventoryItem Item { get; private set; }
        public Quantity ActualQuantity { get; private set; }
        public Quantity ReservedQuantity { get; private set; }
        public Quantity AvailableQuantity { get; private set; }

        private InventoryStockAvailability(InventoryItem item, Quantity actualQuantity, Quantity reservedQuantity, Quantity availableQuantity)
        {
            Item = item;
            ActualQuantity = actualQuantity;
            ReservedQuantity = reservedQuantity;
            AvailableQuantity = availableQuantity;
        }

        public static InventoryStockAvailability Create(InventoryItem item, Quantity actualQuantity, Quantity reservedQuantity, Quantity availableQuantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentNullException.ThrowIfNull(actualQuantity, nameof(actualQuantity));
            ArgumentNullException.ThrowIfNull(reservedQuantity, nameof(reservedQuantity));
            ArgumentNullException.ThrowIfNull(availableQuantity, nameof(availableQuantity));

            if(availableQuantity.Value != actualQuantity.Value - reservedQuantity.Value)
                throw new ArgumentException("Available quantity must be equal to actual quantity minus reserved quantity.");

            return new InventoryStockAvailability(item, actualQuantity, reservedQuantity, availableQuantity);
        }

        public static InventoryStockAvailability Create(InventoryItem item, Quantity actualQuantity, Quantity reservedQuantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentNullException.ThrowIfNull(actualQuantity, nameof(actualQuantity));
            ArgumentNullException.ThrowIfNull(reservedQuantity, nameof(reservedQuantity));

            var availableQuantity = actualQuantity.Subtract(reservedQuantity);

            if(availableQuantity.Value <= 0)
            {
                availableQuantity = Quantity.Create(0m, actualQuantity.Unit);
            }

            return new InventoryStockAvailability(item, actualQuantity, reservedQuantity, availableQuantity);
        }
    }
}
