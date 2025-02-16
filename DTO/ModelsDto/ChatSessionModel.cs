namespace MyShop.DTO.ModelsDto
{
    public class ChatSessionModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public List<MessageModel> Messages { get; set; }
    }
}
