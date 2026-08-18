using Common.Persistence.Concurrency;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryReservationCapacityRepository(InventoryDbContext dbContext) : IInventoryReservationCapacityRepository
    {
        public async Task AddAsync(InventoryReservationCapacity capacity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(capacity);

            await dbContext.InventoryReservationCapacities.AddAsync(capacity.ToEntity(), cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryReservationCapacity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await dbContext.InventoryReservationCapacities
                    .AsNoTracking()
                    .Include(x => x.Item)
                    .ToListAsync(cancellationToken);

            return [.. entities.Select(x => x.ToDomain())];
        }

        public async Task<ConcurrencySnapshot<InventoryReservationCapacity>?> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var entity = await dbContext.InventoryReservationCapacities
                    .AsNoTracking()
                    .Include(x => x.Item)
                    .FirstOrDefaultAsync(x => x.ItemId == itemId, cancellationToken);

            if (entity is null)
                return null;

            return new ConcurrencySnapshot<InventoryReservationCapacity>(entity.ToDomain(), new ConcurrencyToken(entity.RowVersion));
        }

        public async Task UpdateAsync(InventoryReservationCapacity capacity, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(capacity);
            ArgumentNullException.ThrowIfNull(concurrencyToken);

            var entity = await dbContext.InventoryReservationCapacities.FirstOrDefaultAsync(x => x.ItemId == capacity.Item.Id, cancellationToken) ?? throw new InvalidOperationException($"Reservation capacity for item '{capacity.Item.Id}' was not found.");

            entity.UpdateFromDomain(capacity);

            dbContext.Entry(entity).Property(x => x.RowVersion).OriginalValue = concurrencyToken.ToArray();
        }
    }
}
