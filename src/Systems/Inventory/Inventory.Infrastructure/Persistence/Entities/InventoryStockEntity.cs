namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryStockEntity
    {
        public Guid ItemId { get; set; }
        public InventoryItemEntity Item { get; set; } = null!;

        public decimal QuantityValue { get; set; }
        public string QuantityUnit { get; set; } = string.Empty;

        public byte[] RowVersion { get; set; } = [];
    }
}
