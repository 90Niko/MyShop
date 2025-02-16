using System.ComponentModel.DataAnnotations;

namespace MyShop.Data.Models
{
    public class ChatSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
