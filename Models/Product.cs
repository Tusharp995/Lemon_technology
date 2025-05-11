using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public int StockQuantity { get; set; }

        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductImage> Images { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
