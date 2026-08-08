#pragma warning disable CA1716 // Public C# API;
// Optional is intentionally named to represent an optional value, and is not intended to conflict with any other APIs.
namespace Common.Optional
{
    public readonly struct Optional<T>(T? value)
    {
        public bool IsSpecified { get; } = true;
        public T? Value { get; } = value;

        public readonly T ValueOrThrow()
        {
            if (!IsSpecified)
                throw new InvalidOperationException("The optional value has not been set.");

            return Value!;
        }

        public readonly T? GetValueOrDefault(T? defaultValue = default)
        {
            return IsSpecified ? Value : defaultValue;
        }

        public readonly bool TryGetValue(out T? value)
        {
            value = Value;
            return IsSpecified;
        }

        public static implicit operator Optional<T>(T? value) => new(value);
    }
}
#pragma warning restore CA1716
