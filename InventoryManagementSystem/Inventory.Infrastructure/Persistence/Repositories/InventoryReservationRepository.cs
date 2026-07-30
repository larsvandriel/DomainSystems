using Common.Persistence.Concurrency;
using Common.Resilience;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Enums;
using Inventory.Application.Reservations.Services;
using Inventory.Domain.Enums;
using Inventory.Domain.Models;
using Inventory.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class InventoryReservationRepository(InventoryDbContext dbContext, IRetryPolicy retryPolicy) : IInventoryReservationRepository
    {
        private readonly InventoryDbContext _dbContext = dbContext;
        private readonly IRetryPolicy _retryPolicy = retryPolicy;

        public async Task AddAsync(InventoryReservation reservation, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            await _dbContext.InventoryReservations.AddAsync(reservation.ToEntity(), cancellationToken);
        }

        public async Task<IReadOnlyList<InventoryReservation>> GetAsync(ReservationQueryFilter filter, DateTimeOffset activeAt, CancellationToken cancellationToken)
        {
            var query = _dbContext.InventoryReservations.AsNoTracking().Include(x => x.Item).AsQueryable();

            if (filter.ItemId is { } itemId)
            {
                query = query.Where(x => x.ItemId == itemId);
            }

            if (!string.IsNullOrWhiteSpace(filter.ItemName))
            {
                var itemName = filter.ItemName.Trim();
                query = query.Where(x => x.Item.Name.Contains(itemName));
            }

            if (filter.Selection == ReservationSelection.Active)
            {
                query = query.Where(x => x.Status == ReservationStatus.Open && (x.ExpiresAt == null || x.ExpiresAt > activeAt));
            }

            var result = await _retryPolicy.ExecuteAsync(ct => query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).ToListAsync(ct), cancellationToken);

            return result.Select(x => x.ToDomain()).ToList().AsReadOnly();
        }

        public async Task<ConcurrencySnapshot<InventoryReservation>?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken)
        {
            var entity = await _retryPolicy.ExecuteAsync(
                ct => _dbContext.InventoryReservations.AsNoTracking()
                    .Include(x => x.Item)
                    .Where(x => x.Id == reservationId)
                    .FirstOrDefaultAsync(ct),
                cancellationToken);

            if (entity == null)
                return null;

            return new ConcurrencySnapshot<InventoryReservation>(entity.ToDomain(), new ConcurrencyToken(entity.RowVersion));
        }

        public async Task<InventoryReservation?> GetByReference(string reference, CancellationToken cancellationToken)
        {
            var result = await _retryPolicy.ExecuteAsync(
                ct => _dbContext.InventoryReservations.AsNoTracking()
                    .Include(x => x.Item)
                    .Where(x => x.Reference == reference)
                    .FirstOrDefaultAsync(ct),
                cancellationToken);

            return result?.ToDomain();
        }

        public async Task UpdateAsync(InventoryReservation reservation, ConcurrencyToken concurrencyToken, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            var entity = await _dbContext.InventoryReservations.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == reservation.Id, cancellationToken);

            if (entity == null)
                throw new InvalidOperationException($"Inventory reservation for reservation '{reservation.Id}' was not found.");

            entity.UpdateFromDomain(reservation);

            _dbContext.Entry(entity).Property(x => x.RowVersion).OriginalValue = concurrencyToken.ToArray();
        }
    }
}
