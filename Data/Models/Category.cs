using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Data.Models
{
    [Comment("Category of the Product")]
    public class Category
    {
        [Key]
        [Comment("PK of the Category")]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.Constants.CategoryNameMax)]
        [Comment("Name of Category")]
        public string Name { get; set; } = string.Empty;
    }
}
