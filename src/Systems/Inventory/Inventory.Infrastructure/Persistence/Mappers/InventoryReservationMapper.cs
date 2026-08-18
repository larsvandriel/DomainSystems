using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Entities;

namespace Inventory.Infrastructure.Persistence.Mappers
{
    public static class InventoryReservationMapper
    {
        public static InventoryReservation ToDomain(this InventoryReservationEntity entity)
        {
            var item = InventoryItem.Create(entity.Item.Id, entity.Item.Name);
            var quantity = Quantity.Create(entity.QuantityValue, entity.QuantityUnit);

            return InventoryReservation.Restore(
                entity.Id,
                item,
                entity.Status,
                quantity,
                entity.Reference,
                entity.CreatedAt,
                entity.ExpiresAt);
        }

        public static InventoryReservationEntity ToEntity(this InventoryReservation reservation)
        {
            return new InventoryReservationEntity
            {
                Id = reservation.Id,
                Status = reservation.Status,
                ItemId = reservation.Item.Id,
                QuantityUnit = reservation.Quantity.Unit,
                QuantityValue = reservation.Quantity.Value,
                Reference = reservation.Reference,
                CreatedAt = reservation.CreatedAt,
                ExpiresAt = reservation.ExpiresAt
            };
        }

        public static void UpdateFromDomain(this InventoryReservationEntity entity, InventoryReservation reservation)
        {
            entity.QuantityValue = reservation.Quantity.Value;
            entity.QuantityUnit = reservation.Quantity.Unit;

            entity.Reference = reservation.Reference;
            entity.ExpiresAt = reservation.ExpiresAt;

            entity.Status = reservation.Status;
        }
    }
}
