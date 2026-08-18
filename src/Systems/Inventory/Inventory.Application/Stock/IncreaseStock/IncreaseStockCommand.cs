using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Stock.IncreaseStock
{
    public sealed record IncreaseStockCommand(Guid ItemId, string ItemName, decimal Amount, string Unit) : IRequest<Result>;
}
