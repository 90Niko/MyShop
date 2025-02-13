using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Data.Models
{
    public class OrderItem
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("OrderId")]
        public Order Order { get; set; }= null!;

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
    }
}
