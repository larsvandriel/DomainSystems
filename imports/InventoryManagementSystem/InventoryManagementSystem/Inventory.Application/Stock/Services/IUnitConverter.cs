using Inventory.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Stock.Services
{
    public interface IUnitConverter
    {
        bool CanConvert(string sourceUnit, string targetUnit);
        Quantity Convert(Quantity sourceQuantity, string targetUnit);
        bool TryConvert(Quantity sourceQuantity, string targetUnit, out Quantity? convertedQuantity);
    }
}
