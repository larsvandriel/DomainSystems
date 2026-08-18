using Common.Pagination.Pagination;
using Common.Pagination.Sorting;
using Inventory.Application.Reservations.Models;

namespace Inventory.Application.Abstractions
{
    public interface IReservationReadRepository
    {
        Task<PageResult<ReservationSummary>> SearchAsync(
            ReservationFilter filter,
            PageRequest page,
            IReadOnlyList<SortOption<ReservationSortField>> sort,
            DateTimeOffset activeAt,
            CancellationToken cancellationToken = default);
    }
}
