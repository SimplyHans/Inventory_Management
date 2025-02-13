using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Order
{

    [Key]
    public int OrderId { get; set; }
}