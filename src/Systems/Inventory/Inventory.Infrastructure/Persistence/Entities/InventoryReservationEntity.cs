using Inventory.Domain.Enums;

namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryReservationEntity
    {
        public Guid Id { get; set; }
        public ReservationStatus Status { get; set; }

        public Guid ItemId { get; set; }
        public InventoryItemEntity Item { get; set; } = null!;

        public decimal QuantityValue { get; set; }
        public string QuantityUnit { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }

        public byte[] RowVersion { get; set; } = [];
    }
}
