using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Core.Domain
{
    public interface IInventoryItem
    {
        Guid Id { get; }
        string Name { get; }
    }
}
