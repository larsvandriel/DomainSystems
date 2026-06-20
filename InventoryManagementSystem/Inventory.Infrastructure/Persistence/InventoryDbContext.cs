using Inventory.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inventory.Infrastructure.Persistence
{
    public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
    {
        public DbSet<InventoryItemEntity> InventoryItems => Set<InventoryItemEntity>();
        public DbSet<InventoryStockEntity> InventoryStocks => Set<InventoryStockEntity>();
        public DbSet<InventoryMutationEntity> InventoryMutations => Set<InventoryMutationEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}
