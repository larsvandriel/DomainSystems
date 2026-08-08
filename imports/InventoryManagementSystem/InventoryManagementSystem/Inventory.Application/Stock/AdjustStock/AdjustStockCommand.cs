using Common.Messaging.Abstractions.Requests;
using Common.Results;

namespace Inventory.Application.Stock.AdjustStock
{
    public sealed record class AdjustStockCommand(Guid ItemId, string ItemName, decimal Amount, string Unit) : IRequest<Result>;
}
