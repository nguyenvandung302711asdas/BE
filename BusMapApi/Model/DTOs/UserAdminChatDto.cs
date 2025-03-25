namespace BusMapApi.Models.DTOs
{
    public class UserAdminChatDto
    {
        public int UserId { get; set; }
        public int AdminId { get; set; }
        public string Title { get; set; } = "Cuộc trò chuyện với Admin";
    }
}