namespace BusMapApi.Models.DTOs
{
    public class ChatDto
    {
        public int UserId { get; set; }
    }

    public class DetailChatDto
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public Guid ChatId { get; set; }
    }
}