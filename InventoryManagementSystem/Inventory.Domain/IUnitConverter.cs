using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain
{
    public interface IUnitConverter
    {
        bool CanConvert(string unit1, string unit2);
        Quantity Convert(Quantity quantity, string unit);
    }
}
