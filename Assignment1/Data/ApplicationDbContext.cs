using Assignment1.Areas.ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Order = Assignment1.Areas.ProjectManagement.Models.Order;

namespace Assignment1.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Order> Orders { get; set; }
        
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<CartItem> CartItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Ensures Identity's configuration are applied first
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
        
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Grocery" },
            new Category { CategoryId = 2, Name = "Electronic" },
            new Category { CategoryId = 3, Name = "Sports" },
            new Category { CategoryId = 4, Name = "Clothing" }
            );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Apple", Category = "Grocery", Price = 1.50m, Quantity = 100, LowStockThreshold = 10 },
            new Product { Id = 2, Name = "Laptop", Category = "Electronic", Price = 1200.00m, Quantity = 50, LowStockThreshold = 5 },
            new Product { Id = 3, Name = "Football", Category = "Sports", Price = 25.00m, Quantity = 30, LowStockThreshold = 5 },
            new Product { Id = 4, Name = "T-Shirt", Category = "Clothing", Price = 15.00m, Quantity = 200, LowStockThreshold = 20 }
            );
        
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
        });
        
        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Roles");
        });
        
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        
        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
        
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        
        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
    }
    

}