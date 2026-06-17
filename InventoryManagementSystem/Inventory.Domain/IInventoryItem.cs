using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain
{
    public interface IInventoryItem
    {
        Guid Id { get; }
        string Name { get; }
    }
}
