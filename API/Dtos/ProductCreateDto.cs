using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class ProductCreateDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string Thumbnail { get; set; } // The URL from Cloudinary/Imgur
        public int Stock { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; } = "3D-Print-Forge"; // Default brand
    }
}
