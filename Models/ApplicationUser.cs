using Microsoft.AspNetCore.Identity;

namespace Assignment1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PreferredCategory { get; set; }
    }
}