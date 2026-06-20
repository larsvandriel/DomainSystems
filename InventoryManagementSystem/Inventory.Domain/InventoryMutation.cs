namespace Inventory.Domain
{
    public sealed class InventoryMutation
    {
        public InventoryItem Item { get; }
        public Quantity Quantity { get; }
        public InventoryMutationType Type { get; }
        public DateTimeOffset CreatedAt { get; }

        private InventoryMutation(InventoryItem item, Quantity quantity, InventoryMutationType type) : this(item, quantity, type, DateTimeOffset.UtcNow)
        {
        }

        private InventoryMutation(InventoryItem item, Quantity quantity, InventoryMutationType type, DateTimeOffset createdAt)
        {
            Item = item;
            Quantity = quantity;
            Type = type;
            CreatedAt = createdAt;
        }

        public static InventoryMutation CreateIncrease(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(item, quantity, InventoryMutationType.Increase);
        }

        public static InventoryMutation CreateDecrease(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(item, quantity, InventoryMutationType.Decrease);
        }

        public static InventoryMutation CreateAdjustment(InventoryItem item, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(item, quantity, InventoryMutationType.Adjustment);
        }

        public static InventoryMutation Restore(InventoryItem item, Quantity quantity, InventoryMutationType type, DateTimeOffset createdAt)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            ArgumentNullException.ThrowIfNull(quantity, nameof(quantity));

            return new InventoryMutation(item, quantity, type, createdAt);
        }
    }
}
