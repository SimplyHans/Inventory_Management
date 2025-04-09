using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Assignment1.Areas.ProjectManagement.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    public string? PreferredCategories { get; set; }
    
    public bool IsAdmin { get; set; }
    
}