using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Commands
{
    public sealed record IncreaseStockCommand(Guid ItemId, decimal Amount, string Unit) : IRequest<Result> { }
}
