namespace MyShop.DTO.ModelsDto
{
    public class ChatSessionModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }= string.Empty;
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
    }
}
