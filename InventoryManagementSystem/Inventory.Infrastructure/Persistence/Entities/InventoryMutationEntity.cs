using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryMutationEntity
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public InventoryItemEntity Item { get; set; } = null!;

        public decimal? OldQuantityValue { get; set; } = null;
        public string? OldQuantityUnit { get; set; } = null;

        public decimal NewQuantityValue { get; set; }
        public string NewQuantityUnit { get; set; } = string.Empty;

        public InventoryMutationType Type { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
