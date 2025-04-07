using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Match the database column name
        

        public string? Name { get; set; }// Initialize to avoid CS8618 warning
        // Other properties...
    }
}