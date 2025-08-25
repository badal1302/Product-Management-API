using Microsoft.EntityFrameworkCore;
using ProductManagementApi.Models;

namespace ProductManagementApi.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) {}

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, InStock = true },
                new Product { Id = 2, Name = "Mouse", Description = "Wireless optical mouse", Price = 29.99m, InStock = true },
                new Product { Id = 3, Name = "Keyboard", Description = "Mechanical gaming keyboard", Price = 149.99m, InStock = true },
                new Product { Id = 4, Name = "Monitor", Description = "27-inch 4K monitor", Price = 399.99m, InStock = true },
                new Product { Id = 5, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, InStock = false }
            );
        }
    }
}
