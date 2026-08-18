using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations
{
    public sealed class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservationEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryReservationEntity> builder)
        {
            builder.ToTable("InventoryReservations");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);

            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.QuantityValue).HasPrecision(18, 4).IsRequired();
            builder.Property(x => x.QuantityUnit).HasMaxLength(20).IsRequired();

            builder.HasIndex(x => x.Reference).IsUnique();

            builder.Property(x => x.ExpiresAt);

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
