namespace BusMapApi.Model.DTOs
{
    public class GroupChatMessageDto
    {
        public Guid GroupChatId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
    }
}
