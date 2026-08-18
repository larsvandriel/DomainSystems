using Inventory.Domain.Enums;

namespace Inventory.Domain.Models
{
    public sealed class InventoryStock
    {
        public InventoryItem Item { get; }
        public Quantity Quantity { get; private set; }

        private readonly List<InventoryMutation> _mutations = [];

        public IReadOnlyCollection<InventoryMutation> PendingMutations => _mutations.AsReadOnly();

        private InventoryStock(InventoryItem item, Quantity quantity)
        {
            Item = item;
            Quantity = quantity;
        }

        public static InventoryStock Create(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            var stock = new InventoryStock(item, quantity);

            stock._mutations.Add(InventoryMutation.CreateInitial(item, quantity));

            return stock;
        }

        public static InventoryStock Restore(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryStock(item, quantity);
        }

        public void ApplyIncrease(Quantity newQuantity) => ApplyMutation(newQuantity, InventoryMutationType.Increase);

        public void ApplyDecrease(Quantity newQuantity) => ApplyMutation(newQuantity, InventoryMutationType.Decrease);

        public void Adjust(Quantity newQuantity)
        {
            ArgumentNullException.ThrowIfNull(newQuantity);

            if (newQuantity.Value < 0)
                throw new InvalidOperationException("A counted stock quantity cannot be negative.");

            ApplyMutation(newQuantity, InventoryMutationType.Adjustment);
        }

        public void ClearMutations()
        {
            _mutations.Clear();
        }

        private void ApplyMutation(Quantity newQuantity, InventoryMutationType mutationType)
        {
            var previousQuantity = Quantity;

            SetQuantity(newQuantity);

            _mutations.Add(InventoryMutation.Create(Item, previousQuantity, newQuantity, mutationType));
        }
        
        private void SetQuantity(Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            if (!string.Equals(quantity.Unit, Quantity.Unit, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Cannot set quantity with different unit. Current unit: {Quantity.Unit}, New unit: {quantity.Unit}");
            }

            Quantity = quantity;
        }
    }
}
