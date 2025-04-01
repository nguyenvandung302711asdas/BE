using System;
using System.Text.Json.Serialization;

namespace Admin_Bus.Models
{
    public class UserAdminMessage
    {
        public Guid Id { get; set; }

        public Guid ChatId { get; set; }

        public string SenderRole { get; set; } = string.Empty; // "User" hoặc "Admin"

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }

        [JsonIgnore]
        public UserAdminChat? Chat { get; set; }
    }
}