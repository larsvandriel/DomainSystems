using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Mappers
{
    internal static class InventoryReservationCapacityMapper
    {
        public static InventoryReservationCapacity ToDomain(this InventoryReservationCapacityEntity entity)
        {
            var item = InventoryItem.Create(entity.Item.Id, entity.Item.Name);

            var reservedQuantity = Quantity.Create(entity.ReservedQuantityValue, entity.ReservedQuantityUnit);

            return InventoryReservationCapacity.Restore(item, reservedQuantity);
        }

        public static InventoryReservationCapacityEntity ToEntity(this InventoryReservationCapacity capacity)
        {
            return new InventoryReservationCapacityEntity
            {
                ItemId = capacity.Item.Id,
                ReservedQuantityValue = capacity.ReservedQuantity.Value,
                ReservedQuantityUnit = capacity.ReservedQuantity.Unit
            };
        }

        public static void UpdateFromDomain(this InventoryReservationCapacityEntity entity, InventoryReservationCapacity capacity)
        {
            entity.ReservedQuantityValue = capacity.ReservedQuantity.Value;
            entity.ReservedQuantityUnit = capacity.ReservedQuantity.Unit;
        }
    }
}
