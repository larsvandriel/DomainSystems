namespace Inventory.Domain
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
            
            return new Quantity(amount, unit);
        }

        public Quantity Add(Quantity other)
        {
            if(Unit != other.Unit)
            {
                throw new InvalidOperationException($"Cannot sum '{Unit}' and '{other.Unit}' directly.");
            }

            return Create(Value + other.Value, Unit);
        }

        public Quantity Subtract(Quantity other)
        {
            if (Unit != other.Unit)
            {
                throw new InvalidOperationException($"Cannot subtract '{Unit}' and '{other.Unit}' directly.");
            }
            return Create(Value - other.Value, Unit);
        }
    }
}
