using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Domain.Enums
{
    public enum ReservationStatus
    {
        Open,
        Committed,
        Released,
        Expired,
        Canceled
    }
}
