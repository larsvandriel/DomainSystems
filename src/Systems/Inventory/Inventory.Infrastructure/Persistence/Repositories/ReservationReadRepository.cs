using System.Linq.Expressions;
using Common.Pagination.Pagination;
using Common.Pagination.Sorting;
using Common.Persistence.Resilience.Execution;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Enums;
using Inventory.Application.Reservations.Models;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    public sealed class ReservationReadRepository(InventoryDbContext dbContext, IResilientReadExecutor readExecutor) : IReservationReadRepository
    {
        public async Task<PageResult<ReservationSummary>> SearchAsync(
            ReservationFilter filter,
            PageRequest page,
            IReadOnlyList<SortOption<ReservationSortField>> sort,
            DateTimeOffset activeAt,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(page);
            ArgumentNullException.ThrowIfNull(sort);

            IQueryable<InventoryReservationEntity> query = dbContext.InventoryReservations.AsNoTracking();

            query = ApplyFilter(query, filter, activeAt);

            var totalCount = await readExecutor.ExecuteAsync(ct => query.LongCountAsync(ct), cancellationToken);

            query = ApplySorting(query, sort);

            var skip = checked((page.Number - 1) * page.Size);

            var items = await readExecutor.ExecuteAsync(
                ct => query
                    .Skip(skip)
                    .Take(page.Size)
                    .Select(entity => new ReservationSummary(
                        entity.Id,
                        entity.ItemId,
                        entity.Item.Name,
                        entity.QuantityValue,
                        entity.QuantityUnit,
                        entity.Reference,
                        entity.Status,
                        entity.ExpiresAt))
                    .ToListAsync(ct),
                cancellationToken);

            return new PageResult<ReservationSummary>(items, page.Number, page.Size, totalCount);
        }

        private static IQueryable<InventoryReservationEntity> ApplyFilter(
            IQueryable<InventoryReservationEntity> query,
            ReservationFilter filter,
            DateTimeOffset activeAt)
        {
            if (filter.ItemId is { } itemId)
                query = query.Where(entity => entity.ItemId == itemId);

            if (!string.IsNullOrWhiteSpace(filter.ItemName))
            {
                var itemName = filter.ItemName.Trim();

                query = query.Where(entity => entity.Item.Name.Contains(itemName));
            }

            if (filter.Selection == ReservationSelection.Active)
            {
                query = query.Where(
                    entity => entity.Status == ReservationStatus.Open &&
                    (entity.ExpiresAt == null || entity.ExpiresAt > activeAt));
            }

            return query;
        }

        private static IQueryable<InventoryReservationEntity> ApplySorting(
            IQueryable<InventoryReservationEntity> query,
            IReadOnlyList<SortOption<ReservationSortField>> sortOptions)
        {
            IOrderedQueryable<InventoryReservationEntity>? ordered = null;

            foreach (var option in sortOptions)
            {
                ordered = option.Field switch
                {
                    ReservationSortField.CreatedAt => ApplyOrder(query, ordered, entity => entity.CreatedAt, option.Direction),
                    ReservationSortField.ExpiresAt => ApplyOrder(query, ordered, entity => entity.ExpiresAt, option.Direction),
                    ReservationSortField.Reference => ApplyOrder(query, ordered, entity => entity.Reference, option.Direction),
                    _ => throw new ArgumentOutOfRangeException(nameof(sortOptions), option.Field, "Unsupported reservation sort field.")
                };
            }

            if(ordered is null)
            {
                ordered = query.OrderByDescending(entity => entity.CreatedAt).ThenBy(entity => entity.Id);
            }
            else
            {
                ordered = ordered.ThenBy(entity => entity.Id);
            }

            return ordered;
        }

        private static IOrderedQueryable<T> ApplyOrder<T, TKey>(
            IQueryable<T> query,
            IOrderedQueryable<T>? ordered,
            Expression<Func<T, TKey>> selector,
            SortDirection direction)
        {
            if(ordered is null)
            {
                return direction == SortDirection.Ascending ? query.OrderBy(selector) : query.OrderByDescending(selector);
            }

            return direction == SortDirection.Ascending ? ordered.ThenBy(selector) : ordered.ThenByDescending(selector);
        }
    }
}
