namespace Inventory.Domain.Models
{
    public sealed class InventoryReservationCapacity
    {
        public InventoryItem Item { get; }
        public Quantity ReservedQuantity { get; private set; }

        private InventoryReservationCapacity(InventoryItem item, Quantity reservedQuantity)
        {
            Item = item;
            ReservedQuantity = reservedQuantity;
        }

        public static InventoryReservationCapacity Create(InventoryItem item, string unit)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentException.ThrowIfNullOrWhiteSpace(unit);

            return new InventoryReservationCapacity(item, Quantity.Create(0m, unit));
        }

        public static InventoryReservationCapacity Restore(InventoryItem item, Quantity reservedQuantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(reservedQuantity);

            if (reservedQuantity.Value < 0)
                throw new InvalidOperationException("Reserved quantity cannot be negative.");

            return new InventoryReservationCapacity(item, reservedQuantity);
        }

        public void Reserve(Quantity quantity)
        {
            ReservedQuantity = ReservedQuantity.Add(quantity);
        }

        public void Release(Quantity quantity)
        {
            var newQuantity = ReservedQuantity.Subtract(quantity);

            if (newQuantity.Value < 0)
                throw new InvalidOperationException("Cannot release more than the reserved quantity.");

            ReservedQuantity = newQuantity;
        }

        public void Adjust(Quantity previousQuantity, Quantity newQuantity)
        {
            var withoutPrevious = ReservedQuantity.Subtract(previousQuantity);

            if (withoutPrevious.Value < 0)
                throw new InvalidOperationException("The previous reservation exceeds the reserved capacity.");

            ReservedQuantity = withoutPrevious.Add(newQuantity);
        }
    }
}
