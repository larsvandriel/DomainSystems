using Inventory.Domain.Enums;

namespace Inventory.Domain.Models
{
    public sealed class InventoryMutation
    {
        public InventoryItem Item { get; }
        public Quantity? OldQuantity { get; }
        public Quantity NewQuantity { get; }
        public InventoryMutationType Type { get; }
        public DateTimeOffset CreatedAtUtc { get; }

        public Quantity? Difference => OldQuantity is null ? null : NewQuantity.Subtract(OldQuantity);

        private InventoryMutation(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity, InventoryMutationType type) : this(item, oldQuantity, newQuantity, type, DateTimeOffset.UtcNow)
        {
        }

        private InventoryMutation(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity, InventoryMutationType type, DateTimeOffset createdAtUtc)
        {
            Item = item;
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
            Type = type;
            CreatedAtUtc = createdAtUtc;
        }

        public static InventoryMutation Create(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity, InventoryMutationType type)
        {
            return type switch
            {
                InventoryMutationType.Initial => CreateInitial(item, newQuantity),
                InventoryMutationType.Increase => CreateIncrease(item, oldQuantity, newQuantity),
                InventoryMutationType.Decrease => CreateDecrease(item, oldQuantity, newQuantity),
                InventoryMutationType.Adjustment => CreateAdjustment(item, oldQuantity, newQuantity),
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported mutation type: {type}")
            };
        }

        public static InventoryMutation CreateInitial(InventoryItem item, Quantity newQuantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(newQuantity);

            return new InventoryMutation(item, null, newQuantity, InventoryMutationType.Initial);
        }

        public static InventoryMutation CreateIncrease(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(oldQuantity);
            ArgumentNullException.ThrowIfNull(newQuantity);

            if (oldQuantity.Value <= newQuantity.Value)
                throw new InvalidOperationException("Tried to create increasement mutation while new quantity is less or equal to old quantity.");

            return new InventoryMutation(item, oldQuantity, newQuantity, InventoryMutationType.Increase);
        }

        public static InventoryMutation CreateDecrease(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(oldQuantity);
            ArgumentNullException.ThrowIfNull(newQuantity);

            if (oldQuantity.Value >= newQuantity.Value)
                throw new InvalidOperationException("Tried to create decreasement mutation while new quantity is less or equal to old quantity.");

            return new InventoryMutation(item, oldQuantity, newQuantity, InventoryMutationType.Decrease);
        }

        public static InventoryMutation CreateAdjustment(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(oldQuantity);
            ArgumentNullException.ThrowIfNull(newQuantity);

            if (oldQuantity.Value == newQuantity.Value)
                throw new InvalidOperationException("Tried to create adjustment mutation while no adjustment was made.");

            return new InventoryMutation(item, oldQuantity, newQuantity, InventoryMutationType.Adjustment);
        }

        public static InventoryMutation Restore(InventoryItem item, Quantity? oldQuantity, Quantity newQuantity, InventoryMutationType type, DateTimeOffset createdAt)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(newQuantity);

            if (type == InventoryMutationType.Initial && oldQuantity is not null)
                throw new ArgumentException(
                    "An initial mutation cannot have an old quantity.",
                    nameof(oldQuantity));

            return new InventoryMutation(item, oldQuantity, newQuantity, type, createdAt);
        }
    }
}
