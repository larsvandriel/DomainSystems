using Common.Messaging.Abstractions.Requests;
using Common.Results;
using Inventory.Application.Stock.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Commands
{
    public sealed record ApplyStockCountCommand(IReadOnlyCollection<StockCountLine> Lines) : IRequest<Result>;
}
