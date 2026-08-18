using Inventory.Domain.Enums;

namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryMutationEntity
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public InventoryItemEntity Item { get; set; } = null!;

        public decimal? OldQuantityValue { get; set; }
        public string? OldQuantityUnit { get; set; }

        public decimal NewQuantityValue { get; set; }
        public string NewQuantityUnit { get; set; } = string.Empty;

        public InventoryMutationType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
