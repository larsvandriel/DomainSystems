using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Core.Domain
{
    public sealed class InventoryStock
    {
        public required Guid ItemId { get; init; }
        public required Quantity Quantity { get; init; }
    }
}
