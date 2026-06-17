using Common.Messaging.Abstractions.Requests;
using Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Commands
{
    public sealed record DecreaseStockCommand(Guid ItemId, decimal Amount, string Unit) : IRequest<Result> { }
}
