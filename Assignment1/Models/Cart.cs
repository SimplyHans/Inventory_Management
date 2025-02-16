using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; } // Primary key
        public int ProductId { get; set; } // Foreign key to Product
        public int Quantity { get; set; } // Quantity of the product in the cart
        public int? OrderId { get; set; } // Foreign key to Order (nullable)

        // Navigation property to Product
        public Product? Product { get; set; }

        // Navigation property to Order
        public Order? Order { get; set; }

        // Calculated property for total price of the cart item
        public decimal TotalPrice => Product?.Price * Quantity ?? 0;
    }
}