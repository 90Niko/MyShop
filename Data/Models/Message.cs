using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("ChatSession")]
        public int ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;
    }
}
