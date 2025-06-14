// --- FULLY CORRECTED AND FINALIZED FOR COLUMN NAMES: DataAccess/AppDbContext.cs ---
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IMS_Group03.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Entity Configurations

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                // No changes needed here, User keys are usually just 'Id'
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                // No changes needed here
            });

            // --- Product Configuration ---
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                // FIX: Explicitly map the foreign key property to the database column name.
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId");

                entity.HasOne(p => p.Supplier).WithMany(s => s.Products).HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.SetNull);
            });

            // --- PurchaseOrder Configuration ---
            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                // FIX: Explicitly map all foreign key properties.
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId");
                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId");

                entity.HasOne(po => po.Supplier).WithMany(s => s.PurchaseOrders).HasForeignKey(po => po.SupplierId).IsRequired().OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(po => po.CreatedByUser).WithMany(u => u.CreatedPurchaseOrders).HasForeignKey(po => po.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
            });

            // --- PurchaseOrderItem Configuration ---
            modelBuilder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.HasKey(poi => poi.PurchaseOrderItemId);
                entity.HasIndex(poi => new { poi.PurchaseOrderId, poi.ProductId }).IsUnique();

                // FIX: Explicitly map all foreign key properties.
                entity.Property(e => e.PurchaseOrderId).HasColumnName("PurchaseOrderId");
                entity.Property(e => e.ProductId).HasColumnName("ProductId");

                entity.HasOne(poi => poi.PurchaseOrder).WithMany(po => po.PurchaseOrderItems).HasForeignKey(poi => poi.PurchaseOrderId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(poi => poi.Product).WithMany(p => p.PurchaseOrderItems).HasForeignKey(poi => poi.ProductId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            // --- StockMovement Configuration ---
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);

                // FIX: Explicitly map all foreign key properties.
                entity.Property(e => e.ProductId).HasColumnName("ProductId");
                entity.Property(e => e.SourcePurchaseOrderId).HasColumnName("SourcePurchaseOrderId");
                entity.Property(e => e.PurchaseOrderItemId).HasColumnName("PurchaseOrderItemId");
                entity.Property(e => e.PerformedByUserId).HasColumnName("PerformedByUserId");

                entity.HasOne(sm => sm.Product).WithMany(p => p.StockMovements).HasForeignKey(sm => sm.ProductId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(sm => sm.SourcePurchaseOrder).WithMany().HasForeignKey(sm => sm.SourcePurchaseOrderId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
                entity.HasOne(sm => sm.PurchaseOrderItem).WithMany().HasForeignKey(sm => sm.PurchaseOrderItemId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
                entity.HasOne(sm => sm.PerformedByUser).WithMany(u => u.PerformedStockMovements).HasForeignKey(sm => sm.PerformedByUserId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            });

            #endregion
        }
    }
}