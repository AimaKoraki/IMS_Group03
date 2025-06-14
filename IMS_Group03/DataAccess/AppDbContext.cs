// --- FULLY CORRECTED AND FINALIZED: DataAccess/AppDbContext.cs ---
using IMS_Group03.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IMS_Group03.DataAccess
{
    public class AppDbContext : DbContext
    {
        // (DbSet properties are correct and unchanged)
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

            // (User, Supplier, Product, PurchaseOrder, and PurchaseOrderItem configurations are correct and unchanged)
            #region Unchanged Configurations
            modelBuilder.Entity<User>(entity => { /* ... */ });
            modelBuilder.Entity<Supplier>(entity => { /* ... */ });
            modelBuilder.Entity<Product>(entity => { /* ... */ });
            modelBuilder.Entity<PurchaseOrder>(entity => { /* ... */ });
            modelBuilder.Entity<PurchaseOrderItem>(entity => {
                entity.HasKey(poi => poi.PurchaseOrderItemId);
                entity.HasIndex(poi => new { poi.PurchaseOrderId, poi.ProductId }).IsUnique();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
                entity.HasOne(poi => poi.PurchaseOrder).WithMany(po => po.PurchaseOrderItems).HasForeignKey(poi => poi.PurchaseOrderId).IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(poi => poi.Product).WithMany(p => p.PurchaseOrderItems).HasForeignKey(poi => poi.ProductId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });
            #endregion

            // --- StockMovement Configuration ---
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MovementDate).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.HasOne(sm => sm.Product).WithMany(p => p.StockMovements).HasForeignKey(sm => sm.ProductId).IsRequired().OnDelete(DeleteBehavior.Restrict);

                // FIX: This relationship is changed to NoAction to resolve the conflict.
                entity.HasOne(sm => sm.SourcePurchaseOrder).WithMany().HasForeignKey(sm => sm.SourcePurchaseOrderId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);

                entity.HasOne(sm => sm.PurchaseOrderItem).WithMany().HasForeignKey(sm => sm.PurchaseOrderItemId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
                entity.HasOne(sm => sm.PerformedByUser).WithMany(u => u.PerformedStockMovements).HasForeignKey(sm => sm.PerformedByUserId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);
            });
        }
    }
}