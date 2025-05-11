using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? Address { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
