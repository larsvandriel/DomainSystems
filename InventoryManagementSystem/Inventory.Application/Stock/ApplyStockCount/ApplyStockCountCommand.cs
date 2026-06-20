using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.ApplyStockCount
{
    public sealed record ApplyStockCountCommand(IReadOnlyCollection<StockCountLine> Lines) : IRequest<Result>;
}
