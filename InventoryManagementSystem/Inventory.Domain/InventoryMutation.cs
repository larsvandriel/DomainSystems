using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Core.Domain
{
    public sealed class InventoryMutation
    {
        public required Guid ItemId { get; init; }
        public required Quantity Quantity { get; init; }
        public required InventoryMutationType Type { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
