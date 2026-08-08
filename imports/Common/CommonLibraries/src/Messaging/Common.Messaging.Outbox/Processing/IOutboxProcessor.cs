namespace Common.Messaging.Outbox.Processing
{
    public interface IOutboxProcessor
    {
        Task<int> ProcessAsync(CancellationToken cancellationToken = default);
    }
}
