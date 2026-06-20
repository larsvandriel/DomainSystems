using Inventory.Domain;
using Inventory.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Mappers
{
    internal static class InventoryStockMapper
    {
        public static InventoryStock ToDomain(this InventoryStockEntity entity)
        {
            var item = InventoryItem.Create(entity.Item.Id, entity.Item.Name);
            var quantity = Quantity.Create(entity.QuantityValue, entity.QuantityUnit);
            
            return InventoryStock.Restore(item, quantity);
        }

        public static InventoryStockEntity ToEntity(this InventoryStock stock)
        {
            return new InventoryStockEntity
            {
                ItemId = stock.Item.Id,
                Item = stock.Item.ToEntity(),
                QuantityValue = stock.Quantity.Value,
                QuantityUnit = stock.Quantity.Unit
            };
        }

        public static void UpdateFromDomain(this InventoryStockEntity entity, InventoryStock stock)
        {
            entity.QuantityValue = stock.Quantity.Value;
            entity.QuantityUnit = stock.Quantity.Unit;

            entity.Item.Name = stock.Item.Name;
        }
    }
}
