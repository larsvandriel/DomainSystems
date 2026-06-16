namespace Inventory.Core.Domain
{
    public sealed class Quantity
    {
        public required decimal Value { get; init; }
        public required string Unit { get; init; }
    }
}
