namespace Common.Registry
{
    public sealed class Registry<TKey, TValue> : IRegistry<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _items = [];

        public void Register(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            if (!_items.TryAdd(key, value))
                throw new InvalidOperationException($"A registration already exists for key '{key}'.");
        }

        public bool TryGet(TKey key, out TValue? value) => _items.TryGetValue(key, out value);

        public TValue GetRequired(TKey key)
        {
            if(_items.TryGetValue(key, out var value))
                return value;

            throw new KeyNotFoundException($"No registration found for key '{key}'.");
        }
    }
}
