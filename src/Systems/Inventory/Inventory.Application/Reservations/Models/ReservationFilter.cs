using Inventory.Application.Reservations.Enums;

namespace Inventory.Application.Reservations.Models
{
    public sealed record ReservationFilter(
        Guid? ItemId = null,
        string? ItemName = null,
        ReservationSelection Selection = ReservationSelection.All);
}
