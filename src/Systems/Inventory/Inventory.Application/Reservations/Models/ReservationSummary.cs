using Inventory.Domain.Enums;

namespace Inventory.Application.Reservations.Models
{
    public sealed record ReservationSummary(
        Guid Id,
        Guid ItemId,
        string ItemName,
        decimal Amount,
        string Unit,
        string? Reference,
        ReservationStatus Status,
        DateTimeOffset? ExpiresAt);
}
