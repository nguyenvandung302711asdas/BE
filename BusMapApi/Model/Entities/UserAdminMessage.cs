using System.Text.Json.Serialization;

namespace BusMapApi.Model.Entities
{
    public class UserAdminMessage
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        // Navigation property
        [JsonIgnore] // Bỏ qua khi serialize để tránh vòng lặp
        public UserAdminChat Chat { get; set; }
    }
}