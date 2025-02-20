namespace MyShop.DTO.ModelsDto
{
    public class MessageModel
    {
        public int? Id { get; set; }
        public string Sender { get; set; } = string.Empty;  // Who is sending the message
        public string Recipient { get; set; } = string.Empty; // Who is receiving the message
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

}
