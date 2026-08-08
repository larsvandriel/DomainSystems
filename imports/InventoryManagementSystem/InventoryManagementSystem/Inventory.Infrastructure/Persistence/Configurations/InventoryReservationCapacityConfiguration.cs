using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Configurations
{
    public sealed class InventoryReservationCapacityConfiguration : IEntityTypeConfiguration<InventoryReservationCapacityEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryReservationCapacityEntity> builder)
        {
            builder.ToTable("InventoryReservationCapacities");

            builder.HasKey(x => x.ItemId);

            builder.HasOne(x => x.Item).WithOne().HasForeignKey<InventoryReservationCapacityEntity>(x => x.ItemId);

            builder.Property(x => x.ReservedQuantityValue).HasPrecision(18, 4).IsRequired();

            builder.Property(x => x.ReservedQuantityUnit).HasMaxLength(20).IsRequired();

            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}
