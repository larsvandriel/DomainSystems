using Inventory.Domain;
using Inventory.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Mappers
{
    internal static class InventoryMutationMapper
    {
        public static InventoryMutation ToDomain(this InventoryMutationEntity entity)
        {
            var item = InventoryItem.Create(entity.Item.Id, entity.Item.Name);
            var quantity = Quantity.Create(entity.QuantityValue, entity.QuantityUnit);

            return InventoryMutation.Restore(item, quantity, entity.Type, entity.CreatedAt);
        }
        
        public static InventoryMutationEntity ToEntity(this InventoryMutation mutation)
        {
            return new InventoryMutationEntity
            {
                ItemId = mutation.Item.Id,
                QuantityValue = mutation.Quantity.Value,
                QuantityUnit = mutation.Quantity.Unit,
                Type = mutation.Type,
                CreatedAt = mutation.CreatedAt,
            };
        }
    }
}
