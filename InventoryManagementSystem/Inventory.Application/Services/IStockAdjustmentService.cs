using Inventory.Application.Stock.ApplyStockCount;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Services
{
    public interface IStockAdjustmentService
    {
        Task ApplyAsync(StockCountLine line, CancellationToken cancellationToken);
    }
}
