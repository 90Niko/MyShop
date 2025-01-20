using System.ComponentModel.DataAnnotations;

namespace MyShop.DTO.ModelsDto
{
    public class CategoryModel
    {
        [Required]
        [MinLength(Constants.Constants.CategoryNameMin)]
        [MaxLength(Constants.Constants.CategoryNameMax)]    
        public string Name { get; set; } = string.Empty;
    }
}
