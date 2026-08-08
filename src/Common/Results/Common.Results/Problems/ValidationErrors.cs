namespace Common.Results.Problems
{
    public sealed class ValidationErrors
    {
        private readonly Dictionary<string, List<string>> _errors = [];

        public bool Any => _errors.Count > 0;

        public void Add(string key, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            if (!_errors.TryGetValue(key, out var messages))
            {
                messages = [];
                _errors[key] = messages;
            }

            if (!messages.Contains(message, StringComparer.Ordinal))
                messages.Add(message);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> errors)
        {
            ArgumentNullException.ThrowIfNull(errors);

            foreach (var error in errors)
                Add(error.Key, error.Value);
        }

        public IReadOnlyDictionary<string, string[]> ToDictionary() =>
            _errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }
}
