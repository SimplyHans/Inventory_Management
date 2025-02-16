using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Assignment1.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; } // Primary key

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Default to current UTC time

        // Navigation property for Cart items in the order
        public List<Cart> CartItems { get; set; } = new List<Cart>();

        // Navigation property for OrderList items
        public List<OrderList> OrderLists { get; set; } = new List<OrderList>();

        // Calculated property for totdotnet ef migrations add AddOrderListTableal quantity (in-memory calculation)
        public int TotalQuantity => CartItems?.Sum(item => item.Quantity) ?? 0;

        // Calculated property for total price (in-memory calculation)
        public decimal TotalPrice => CartItems?.Sum(item => item.TotalPrice) ?? 0;
    }
}