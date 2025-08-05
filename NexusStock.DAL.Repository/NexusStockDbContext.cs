using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusStock.Common.Entities;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository
{
    public class NexusStockDbContext : DbContext
    {
        public NexusStockDbContext(DbContextOptions<NexusStockDbContext> options) : base(options) { }

        public DbSet<Resource> Resources { get; set; } = null!;
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<StockBalance> StockBalances { get; set; } = null!;
        public DbSet<ReceiptDocument> ReceiptDocuments { get; set; } = null!;
        public DbSet<ReceiptItem> ReceiptItems { get; set; } = null!;
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; } = null!;
        public DbSet<ShipmentItem> ShipmentItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureResource(modelBuilder);
            ConfigureUnit(modelBuilder);
            ConfigureClient(modelBuilder);
            ConfigureStockBalance(modelBuilder);
            ConfigureReceiptDocument(modelBuilder);
            ConfigureReceiptItem(modelBuilder);
            ConfigureShipmentDocument(modelBuilder);
            ConfigureShipmentItem(modelBuilder);
        }

        private void ConfigureResource(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(r => r.Name).IsUnique();
                entity.Property(r => r.IsActive).HasDefaultValue(true);
            });
        }

        private void ConfigureUnit(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Unit>(entity =>
            {
                entity.HasKey(u => u.Id);
                ConfigureRequiredUniqueProperty(entity, u => u.Name, 50);
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });
        }

        private void ConfigureClient(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);
                ConfigureRequiredUniqueProperty(entity, c => c.Name, 100);
                entity.Property(c => c.Address).HasMaxLength(200);
                entity.Property(c => c.IsActive).HasDefaultValue(true);
            });
        }

        private void ConfigureStockBalance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockBalance>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Quantity).HasColumnType("decimal(18,2)");

                entity.HasIndex(b => new { b.ResourceId, b.UnitId })
                    .IsUnique();

                // Внешние ключи
                entity.HasOne(s => s.Resource)
                    .WithMany(r => r.Balances)
                    .HasForeignKey(s => s.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Unit)
                    .WithMany(u => u.Balances)
                    .HasForeignKey(s => s.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureReceiptDocument(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReceiptDocument>(entity =>
            {
                entity.HasKey(r => r.Id);
                ConfigureRequiredUniqueProperty(entity, r => r.Number, 50);
            });
        }

        private void ConfigureReceiptItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReceiptItem>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Quantity).HasColumnType("decimal(18,2)");

                // Внешние ключи с явным указанием навигационных свойств
                entity.HasOne(r => r.ReceiptDocument)
                    .WithMany(d => d.Items)
                    .HasForeignKey(r => r.ReceiptDocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Resource)
                    .WithMany(r => r.ReceiptItems)
                    .HasForeignKey(r => r.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Unit)
                    .WithMany(u => u.ReceiptItems)
                    .HasForeignKey(r => r.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureShipmentDocument(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShipmentDocument>(entity =>
            {
                entity.HasKey(s => s.Id);
                ConfigureRequiredUniqueProperty(entity, s => s.Number, 50);
                entity.Property(s => s.IsSigned).HasDefaultValue(false);

                entity.HasOne(s => s.Client)
                    .WithMany(c => c.Shipments)
                    .HasForeignKey(s => s.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureShipmentItem(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShipmentItem>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Quantity).HasColumnType("decimal(18,2)");

                // Внешние ключи с явным указанием навигационных свойств
                entity.HasOne(s => s.ShipmentDocument)
                    .WithMany(d => d.Items)
                    .HasForeignKey(s => s.ShipmentDocumentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Resource)
                    .WithMany(r => r.ShipmentItems)
                    .HasForeignKey(s => s.ResourceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Unit)
                    .WithMany(u => u.ShipmentItems)
                    .HasForeignKey(s => s.UnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        /// <summary>
        /// Конфигурирует обязательное уникальное свойство сущности с максимальной длиной.
        /// </summary>
        private void ConfigureRequiredUniqueProperty<T>(
            EntityTypeBuilder<T> entity,
            Expression<Func<T, object?>> propertyExpression,
            int maxLength = 100) where T : class
        {
            entity.Property(propertyExpression)
                .IsRequired()
                .HasMaxLength(maxLength);

            entity.HasIndex(propertyExpression).IsUnique();
        }
    }
}
