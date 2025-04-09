using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.ProjectManagement.Models;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; }
}