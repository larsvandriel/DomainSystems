using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryReservationCapacityEntity
    {
        public Guid ItemId { get; set; }
        public InventoryItemEntity Item { get; set; } = null!;

        public decimal ReservedQuantityValue { get; set; }
        public string ReservedQuantityUnit { get; set; } = string.Empty;

        public byte[] RowVersion { get; set; } = [];
    }
}
