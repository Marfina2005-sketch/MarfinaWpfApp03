using Microsoft.EntityFrameworkCore;
using MarfinaLibrary.Models;

namespace MarfinaLibrary.Database
{
    public class MarfinaDbContext : DbContext
    {
        public DbSet<PartnerType> PartnerTypesMarfina { get; set; }
        public DbSet<Partner> PartnersMarfina { get; set; }
        public DbSet<Product> ProductsMarfina { get; set; }
        public DbSet<SalesHistory> SalesHistoriesMarfina { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к PostgreSQL
            optionsBuilder.UseNpgsql("Host=localhost;Database=marfina_db;Username=app;Password=123456789");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка индексов с HasName() для EF Core 
            modelBuilder.Entity<Partner>()
                .HasIndex(p => p.TypeId)
                .HasName("idx_partners_marfina_type");

            modelBuilder.Entity<SalesHistory>()
                .HasIndex(s => s.PartnerId)
                .HasName("idx_sales_history_marfina_partner");

            modelBuilder.Entity<SalesHistory>()
                .HasIndex(s => s.ProductId)
                .HasName("idx_sales_history_marfina_product");

            modelBuilder.Entity<SalesHistory>()
                .HasIndex(s => s.SaleDate)
                .HasName("idx_sales_history_marfina_date");

            // Настройка отношений 
            modelBuilder.Entity<Partner>()
                .HasOne(p => p.PartnerType)
                .WithMany(pt => pt.Partners)
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(s => s.Partner)
                .WithMany(p => p.SalesHistories)
                .HasForeignKey(s => s.PartnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(s => s.Product)
                .WithMany(p => p.SalesHistories)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}