using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.ProjectManagement.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }

    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}