using Common.Persistence.Concurrency;
using Inventory.Application.Abstractions;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryReservationRepository(InventoryDbContext dbContext) : IInventoryReservationRepository
    {
        private readonly InventoryDbContext _dbContext = dbContext;

        public async Task AddAsync(InventoryReservation reservation, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            await _dbContext.InventoryReservations.AddAsync(reservation.ToEntity(), cancellationToken);
        }

        public async Task<ConcurrencySnapshot<InventoryReservation>?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.InventoryReservations.AsNoTracking()
                    .Include(x => x.Item)
                    .Where(x => x.Id == reservationId)
                    .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                return null;

            return new ConcurrencySnapshot<InventoryReservation>(entity.ToDomain(), new ConcurrencyToken(entity.RowVersion));
        }

        public async Task<InventoryReservation?> GetByReference(string reference, CancellationToken cancellationToken)
        {
            var result = await _dbContext.InventoryReservations.AsNoTracking()
                    .Include(x => x.Item)
                    .Where(x => x.Reference == reference)
                    .FirstOrDefaultAsync(cancellationToken);

            return result?.ToDomain();
        }


        public async Task UpdateAsync(InventoryReservation reservation, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            var entity = await _dbContext.InventoryReservations.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == reservation.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Inventory reservation for reservation '{reservation.Id}' was not found.");

            entity.UpdateFromDomain(reservation);

            _dbContext.Entry(entity).Property(x => x.RowVersion).OriginalValue = concurrencyToken.ToArray();
        }
    }
}
