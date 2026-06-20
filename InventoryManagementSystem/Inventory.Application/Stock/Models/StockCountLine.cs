using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Models
{
    public sealed record StockCountLine(Guid ItemId, string ItemName, decimal CountedAmount, string Unit);
}
