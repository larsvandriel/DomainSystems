using Common.Messaging.Abstractions.Requests;
using Common.Pagination.Pagination;
using Common.Results;
using Inventory.Application.Abstractions;
using Inventory.Application.Reservations.Models;

namespace Inventory.Application.Reservations.SearchReservation
{
    public sealed class SearchReservationsQueryHandler(
        IReservationReadRepository repository) : IRequestHandler<SearchReservationsQuery, Result<PageResult<ReservationSummary>>>
    {
        public async Task<Result<PageResult<ReservationSummary>>> HandleAsync(
            SearchReservationsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await repository.SearchAsync(request.Filter, request.Page, request.Sort, DateTimeOffset.UtcNow, cancellationToken);

            return Result.Success(result);
        }
    }
}
