namespace Common.Messaging.Integration.Contracts
{
    public sealed record IntegrationEventEnvelope
    {
        public required Guid MessageId { get; init; }
        public required string Type { get; init; }
        public required int Version { get; init; }
        public required DateTimeOffset OccurredAtUtc { get; init; }
        public required string Payload { get; init; }
        public string? CorrelationId { get; init; }
        public string? CausationId { get; init; }
        public string? TraceId { get; init; }

        public string Identifier => $"{Type}.v{Version}";
    }
}
