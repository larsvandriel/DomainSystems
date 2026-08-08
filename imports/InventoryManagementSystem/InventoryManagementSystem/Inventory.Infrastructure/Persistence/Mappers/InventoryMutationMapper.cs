using Inventory.Domain.Models;
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

            Quantity? oldQuantity = null;

            if (entity.OldQuantityValue is not null && entity.OldQuantityUnit is not null)
            {
                oldQuantity = Quantity.Create(entity.OldQuantityValue.Value, entity.OldQuantityUnit);
            }

            var newQuantity = Quantity.Create(entity.NewQuantityValue, entity.NewQuantityUnit);

            return InventoryMutation.Restore(item, oldQuantity, newQuantity, entity.Type, entity.CreatedAt);
        }
        
        public static InventoryMutationEntity ToEntity(this InventoryMutation mutation)
        {
            return new InventoryMutationEntity
            {
                ItemId = mutation.Item.Id,
                OldQuantityValue = mutation.OldQuantity?.Value,
                OldQuantityUnit = mutation.OldQuantity?.Unit,
                NewQuantityValue = mutation.NewQuantity.Value,
                NewQuantityUnit = mutation.NewQuantity.Unit,
                Type = mutation.Type,
                CreatedAt = mutation.CreatedAtUtc,
            };
        }
    }
}
