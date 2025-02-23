namespace MyShop.DTO.ModelsDto
{
    public class MessageModel
    {
        public string Sender { get; set; } = string.Empty;  // Who is sending the message
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime Timestamp { get; set; }
    }
}
