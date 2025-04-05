using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.ProjectManagement.Models;


    public class Product
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(0.01, int.MaxValue, ErrorMessage = "Price must be a positive integer.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        public int Quantity { get; set; }

        [Required]
        public int LowStockThreshold { get; set; }
    }
