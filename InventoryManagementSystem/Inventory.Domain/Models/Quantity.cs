namespace Inventory.Domain.Models
{
    public sealed class Quantity
    {
        public decimal Value { get; }
        public string Unit { get; }

        private Quantity(decimal value, string unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Quantity Create(decimal amount, string unit)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(unit, nameof(unit));
            
            return new Quantity(amount, unit.Trim().ToLowerInvariant());
        }

        public Quantity Add(Quantity other)
        {
            ArgumentNullException.ThrowIfNull(other, nameof(other));

            if(!Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot sum '{Unit}' and '{other.Unit}' directly.");

            if (other.Value < 0)
                throw new InvalidOperationException("Cannot add negative quantity.");

            return Create(Value + other.Value, Unit);
        }

        public Quantity Subtract(Quantity other)
        {
            ArgumentNullException.ThrowIfNull(other, nameof(other));

            if (!Unit.Equals(other.Unit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Cannot subtract '{Unit}' and '{other.Unit}' directly.");
            
            if(other.Value < 0)
                throw new InvalidOperationException("Cannot subtract negative quantity.");

            return Create(Value - other.Value, Unit);
        }

        public bool IsGreaterThanOrSame(Quantity? other)
        {
            if (other is null)
                ArgumentNullException.ThrowIfNull(other);

            return this.Value >= other.Value;
        }
    }
}
