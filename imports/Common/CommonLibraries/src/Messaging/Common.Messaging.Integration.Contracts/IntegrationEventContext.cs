namespace Common.Messaging.Integration.Contracts
{
    public sealed record IntegrationEventContext
    {
        public string? CorrelationId { get; init; }
        public string? CausationId { get; init; }
        public string? TraceId { get; init; }
    }
}
