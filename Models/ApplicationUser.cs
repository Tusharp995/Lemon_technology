using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Web_Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; } 
    }

}
