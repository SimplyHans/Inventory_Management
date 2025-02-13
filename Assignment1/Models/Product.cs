using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
}