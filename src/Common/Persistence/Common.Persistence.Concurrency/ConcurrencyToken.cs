namespace Common.Persistence.Concurrency
{
    public sealed class ConcurrencyToken
    {
        private readonly byte[] _value;

        public ConcurrencyToken(byte[] value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if(value.Length == 0)
            {
                throw new ArgumentException("Concurrency token value cannot be empty.", nameof(value));
            }

            _value = [.. value];
        }

        public byte[] ToArray() => [.. _value];
    }
}
