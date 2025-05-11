namespace E_Commerce_Web_Application.Models
{
    public class ProductImage
    {
        public int Id { get; set; } // EF will use this as the primary key
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}