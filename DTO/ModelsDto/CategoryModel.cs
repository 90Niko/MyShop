using System.ComponentModel.DataAnnotations;

namespace MyShop.DTO.ModelsDto
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(Constants.Constants.CategoryNameMin)]
        [MaxLength(Constants.Constants.CategoryNameMax)]    
        public string Name { get; set; } = string.Empty;
    }
}
