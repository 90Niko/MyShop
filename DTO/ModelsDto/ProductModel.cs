using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace MyShop.DTO.ModelsDto
{
    public class ProductModel
    {
        [Required]
        [MinLength(Constants.Constants.ProductNameMin)]
        [MaxLength(Constants.Constants.ProductNameMax)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [MinLength(Constants.Constants.ProductDescriptionMin)]
        [MaxLength(Constants.Constants.ProductDescriptionMax)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; }= string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; }


    }
}
