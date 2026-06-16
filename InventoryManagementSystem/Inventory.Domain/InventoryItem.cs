using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Core.Domain
{
    public sealed class InventoryItem : IInventoryItem
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
