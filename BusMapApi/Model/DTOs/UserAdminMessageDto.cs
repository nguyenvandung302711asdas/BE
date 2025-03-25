namespace BusMapApi.Models.DTOs
{
    public class UserAdminMessageDto
    {
        public Guid ChatId { get; set; }
        public string SenderRole { get; set; } = string.Empty; // "User" hoặc "Admin"
        public string Content { get; set; } = string.Empty;
    }
}