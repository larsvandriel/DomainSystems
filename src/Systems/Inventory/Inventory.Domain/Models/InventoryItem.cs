namespace Inventory.Domain.Models
{
    public sealed class InventoryItem
    {
        public const int NameMaxLength = 100;

        public Guid Id { get; }
        public string Name { get; }

        private InventoryItem(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public static InventoryItem Create(Guid id, string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            
            if(id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            }

            if(name.Length > NameMaxLength)
            {
                throw new ArgumentException($"Name cannot exceed {NameMaxLength} characters.", nameof(name));
            }

            return new InventoryItem(id, name);
        }
    }
}
