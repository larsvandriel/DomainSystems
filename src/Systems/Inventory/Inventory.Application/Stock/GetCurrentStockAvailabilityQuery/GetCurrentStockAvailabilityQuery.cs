using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetCurrentStockAvailabilityQuery
{
    public sealed record GetCurrentStockAvailabilityQuery : IRequest<Result<IReadOnlyList<InventoryStockAvailability>>>;
}
