using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Configurations
{
    public sealed class InventoryMutationConfiguration : IEntityTypeConfiguration<InventoryMutationEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryMutationEntity> builder)
        {
            builder.ToTable("InventoryMutations");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId);

            builder.Property(x => x.OldQuantityValue).HasPrecision(18, 4);

            builder.Property(x => x.OldQuantityUnit).HasMaxLength(20);

            builder.Property(x => x.NewQuantityValue).HasPrecision(18, 4).IsRequired();

            builder.Property(x => x.NewQuantityUnit).HasMaxLength(20).IsRequired();

            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasIndex(x => x.ItemId);
        }
    }
}
