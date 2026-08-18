namespace Common.Caching
{
    public interface ICache
    {
        ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        ValueTask SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default);

        ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
