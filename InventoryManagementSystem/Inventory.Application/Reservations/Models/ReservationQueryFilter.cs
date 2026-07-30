using Inventory.Application.Reservations.Enums;
using Inventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Application.Reservations.Services
{
    public sealed record ReservationQueryFilter(
        Guid? ItemId = null,
        string? ItemName = null,
        ReservationSelection Selection = ReservationSelection.All);
}
