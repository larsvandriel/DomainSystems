using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Entities
{
    public sealed class InventoryItemEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
