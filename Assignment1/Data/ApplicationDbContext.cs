using Microsoft.EntityFrameworkCore;
using Assignment1.Models;

namespace Assignment1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet properties
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<OrderList> OrderLists { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if product is deleted

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Order)
                .WithMany(o => o.CartItems)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete if order is deleted

            // Configure OrderList relationships
            modelBuilder.Entity<OrderList>()
                .HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderList>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLists)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Grocery" },
                new Category { CategoryId = 2, Name = "Electronic" },
                new Category { CategoryId = 3, Name = "Sports" },
                new Category { CategoryId = 4, Name = "Clothing" }
            );

            // Seed products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Apple", Category = "Grocery", Price = 1.50m, Quantity = 100, LowStockThreshold = 10 },
                new Product { Id = 2, Name = "Laptop", Category = "Electronic", Price = 1200.00m, Quantity = 50, LowStockThreshold = 5 },
                new Product { Id = 3, Name = "Football", Category = "Sports", Price = 25.00m, Quantity = 30, LowStockThreshold = 5 },
                new Product { Id = 4, Name = "T-Shirt", Category = "Clothing", Price = 15.00m, Quantity = 200, LowStockThreshold = 20 }
            );

            // Seed orders with static OrderDate
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, OrderDate = new DateTime(2023, 10, 1) } // Static value
            );

            // Seed cart data
            modelBuilder.Entity<Cart>().HasData(
                new Cart { Id = 1, ProductId = 1, OrderId = 1, Quantity = 2 },
                new Cart { Id = 2, ProductId = 2, OrderId = 1, Quantity = 1 }
            );

            // Seed OrderList data (optional)
            modelBuilder.Entity<OrderList>().HasData(
                new OrderList { OrderListId = 1, ProductId = 1, OrderId = 1, Quantity = 2 },
                new OrderList { OrderListId = 2, ProductId = 2, OrderId = 1, Quantity = 1 }
            );
        }
    }
}