namespace Common.Messaging.Integration.Contracts
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IntegrationEventTypeAttribute : Attribute
    {
        public string Name { get; }

        public int Version { get; }

        public string Identifier => $"{Name}.v{Version}";

        public IntegrationEventTypeAttribute(string name, int version)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);

            Name = name;
            Version = version;
        }
    }
}
