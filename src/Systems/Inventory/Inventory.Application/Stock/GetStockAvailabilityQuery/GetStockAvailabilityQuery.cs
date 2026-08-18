using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Domain.Models;

namespace Inventory.Application.Stock.GetStockAvailabilityQuery
{
    public sealed record GetStockAvailabilityQuery(Guid ItemId) : IRequest<Result<InventoryStockAvailability>>;
}
