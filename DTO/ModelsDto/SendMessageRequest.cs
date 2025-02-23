namespace MyShop.DTO.ModelsDto
{
    public class SendMessageRequest
    {
        public int ChatSessionId { get; set; }
        public string UserEmail { get; set; }
        public string Message { get; set; }
    }
}
