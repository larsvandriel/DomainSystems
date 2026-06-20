using Inventory.Domain;
using Inventory.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Mappers
{
    public static class InventoryItemMapper
    {
        public static InventoryItem ToDomain(this InventoryItemEntity entity)
        {
            return InventoryItem.Create(entity.Id, entity.Name);
        }

        public static InventoryItemEntity ToEntity(this InventoryItem item)
        {
            return new InventoryItemEntity
            {
                Id = item.Id,
                Name = item.Name,
            };
        }
    }
}
