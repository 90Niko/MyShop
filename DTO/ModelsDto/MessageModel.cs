namespace MyShop.DTO.ModelsDto
{
    public class MessageModel
    {
        public int? Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
