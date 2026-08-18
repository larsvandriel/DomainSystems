using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Stock.DecreaseStock
{
    public sealed record DecreaseStockCommand(Guid ItemId, string ItemName, decimal Amount, string Unit) : IRequest<Result>;
}
