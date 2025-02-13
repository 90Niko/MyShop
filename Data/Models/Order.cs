using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Data.Models
{
    public class Order
    {
        [Key]
        [Comment("PK of Order")]
        public int Id { get; set; }

        [Required]
        [Comment("Date of Order")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Comment("Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Comment("Customer Address")]
        public string CustomerAddress { get; set; } = string.Empty;

        [Required]
        [Comment("Customer Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Comment("Customer Phone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required]
        [Comment("Order Status")]
        public string Status { get; set; } = string.Empty;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
