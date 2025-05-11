using Microsoft.AspNetCore.Identity;

namespace E_Commerce_Web_Application.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } // FK to IdentityUser
        public int Rating { get; set; } // 1 to 5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
