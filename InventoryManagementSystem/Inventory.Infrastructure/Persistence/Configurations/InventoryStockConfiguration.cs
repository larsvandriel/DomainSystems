using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence.Configurations
{
    public sealed class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStockEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryStockEntity> builder)
        {
            builder.ToTable("InventoryStocks");

            builder.HasKey(x => x.ItemId);

            builder.HasOne(x => x.Item).WithOne().HasForeignKey<InventoryStockEntity>(x => x.ItemId);

            builder.Property(x => x.QuantityValue).HasPrecision(18, 4).IsRequired();

            builder.Property(x => x.QuantityUnit).HasMaxLength(20).IsRequired();
        }
    }
}
