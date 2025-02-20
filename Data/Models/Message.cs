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
        public string Sender { get; set; } = string.Empty;

        [Required]
        [Comment("Recipient of Message")]
        public string Recipient { get; set; } = string.Empty; // The user receiving the message

        [Required]
        [Comment("Content of Message")]
        public string Content { get; set; } = string.Empty; // Message text

        [Required]
        public DateTime Timestamp { get; set; }

        [ForeignKey("ChatSession")]
        public int ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;
    }

}
