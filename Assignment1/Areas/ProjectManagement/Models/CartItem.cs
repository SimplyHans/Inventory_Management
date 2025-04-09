using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.ProjectManagement.Models;

public class CartItem
{
    [Key]
    public int Id { get; set; }
    
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;
}