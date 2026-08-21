using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Entities;

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
                QuantityValue = stock.Quantity.Value,
                QuantityUnit = stock.Quantity.Unit
            };
        }

        public static void UpdateFromDomain(this InventoryStockEntity entity, InventoryStock stock)
        {
            entity.QuantityValue = stock.Quantity.Value;
            entity.QuantityUnit = stock.Quantity.Unit;
        }
    }
}
