using Common.Messaging.Abstractions.Requests.Queries;
using Common.Pagination.Pagination;
using Common.Pagination.Sorting;
using Common.Results;
using Inventory.Application.Reservations.Models;

namespace Inventory.Application.Reservations.SearchReservation
{
    public sealed record SearchReservationsQuery(
        ReservationFilter Filter,
        PageRequest Page,
        IReadOnlyList<SortOption<ReservationSortField>> Sort) : IQuery<Result<PageResult<ReservationSummary>>>;
}
