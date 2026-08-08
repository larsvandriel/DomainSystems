using Inventory.Domain.Enums;

namespace Inventory.Domain.Models
{
    public sealed class InventoryReservation
    {
        public Guid Id { get; private set; }
        public ReservationStatus Status { get; private set; }
        public InventoryItem Item { get; private set; }
        public Quantity Quantity { get; private set; }
        public string Reference { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? ExpiresAt { get; private set; }

        private InventoryReservation(
            Guid id,
            InventoryItem item,
            ReservationStatus status,
            Quantity quantity,
            string reference,
            DateTimeOffset createdAt,
            DateTimeOffset? expiresAt)
        {
            Id = id;
            Status = status;
            Item = item;
            Quantity = quantity;
            Reference = reference;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }

        public static InventoryReservation Create(InventoryItem item, Quantity quantity, string? reference, DateTimeOffset? expiresAt)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(quantity);

            if (quantity.Value <= 0)
                throw new InvalidOperationException("Cannot create reservation for negative or empty amount.");

            if (expiresAt is not null && expiresAt <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Cannot create reservation when expire time is in the past.");

            var id = Guid.NewGuid();
            reference ??= id.ToString();

            return new InventoryReservation(id, item, ReservationStatus.Open, quantity, reference, DateTimeOffset.UtcNow, expiresAt);
        }

        public static InventoryReservation Restore(
            Guid id,
            InventoryItem item,
            ReservationStatus status,
            Quantity quantity,
            string reference,
            DateTimeOffset createdAt,
            DateTimeOffset? expiresAt)
        {
            ArgumentNullException.ThrowIfNull(item);
            ArgumentNullException.ThrowIfNull(quantity);

            if (quantity.Value <= 0)
                throw new InvalidOperationException("Cannot create reservation for negative or empty amount.");

            return new InventoryReservation(id, item, status, quantity, reference, createdAt, expiresAt);
        }

        public void Commit()
        {
            EnsureOpenAndNotExpired();

            Status = ReservationStatus.Committed;
        }

        public void Release()
        {
            EnsureOpenAndNotExpired();

            Status = ReservationStatus.Released;
        }

        public void Expire(DateTimeOffset now)
        {
            if (Status != ReservationStatus.Open)
                throw new InvalidOperationException("Only open reservations can expire.");

            if (ExpiresAt is null || ExpiresAt > now)
                throw new InvalidOperationException("Reservation has not expired.");

            Status = ReservationStatus.Expired;
        }

        public void Cancel()
        {
            EnsureOpenAndNotExpired();

            Status = ReservationStatus.Canceled;
        }

        public void AdjustAmount(Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(quantity);

            EnsureOpenAndNotExpired();

            if (quantity.Value <= 0)
                throw new InvalidOperationException("Cannot create reservation for negative or empty amount.");

            Quantity = quantity;
        }

        public void AdjustExpiresAt(DateTimeOffset? expiresAt)
        {
            EnsureOpenAndNotExpired();

            if (expiresAt != null && expiresAt <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Cannot adjust reservation when new expire time is in the past.");

            ExpiresAt = expiresAt;
        }

        public void AdjustReference(string reference)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(reference);

            EnsureOpenAndNotExpired();

            Reference = reference;
        }

        private void EnsureOpenAndNotExpired()
        {
            if (Status != ReservationStatus.Open)
                throw new InvalidOperationException("Reservation not open for adjustments");

            if (ExpiresAt != null && ExpiresAt <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Reservation already expired.");
        }
    }
}
