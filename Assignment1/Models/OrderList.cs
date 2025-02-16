using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class OrderList
    {
        [Key]
        public int OrderListId { get; set; } // Primary key
        public int ProductId { get; set; } // Foreign key to Product
        public int Quantity { get; set; } // Quantity of the product in the order
        public int? OrderId { get; set; } // Foreign key to Order (nullable)

        // Navigation properties
        public Product? Product { get; set; } // Reference to the Product entity
        public Order? Order { get; set; } // Reference to the Order entity
    }
}