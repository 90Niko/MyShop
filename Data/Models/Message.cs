using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Data.Models
{
    public class Message
    {
        [Key]
        [Comment("PK of Message")]
        public int Id { get; set; }  // Auto-generated ID

        [Required]
        [Comment("Sender of Message")]
        public string Sender { get; set; }=string.Empty;

        [Required]
        [Comment("Receiver of Message")]
        public string Content { get; set; }= string.Empty;

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
