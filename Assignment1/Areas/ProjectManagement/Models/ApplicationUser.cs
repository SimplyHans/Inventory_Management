using Microsoft.AspNetCore.Identity;

namespace Assignment1.Areas.ProjectManagement.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? PreferredCategories { get; set; }
    
}