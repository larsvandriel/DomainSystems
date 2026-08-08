namespace Common.Persistence.Concurrency
{
    public sealed record ConcurrencySnapshot<T>(T Value, ConcurrencyToken Token);
}
