using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusMapApi.Model.Entities
{
    public class UserAdminChat
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int AdminId { get; set; }
        public string Title { get; set; } = "Cuộc trò chuyện với Admin";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Navigation properties
        [JsonIgnore] // Bỏ qua khi serialize để tránh vòng lặp
        public Account User { get; set; }

        [JsonIgnore] // Bỏ qua khi serialize để tránh vòng lặp
        public Account Admin { get; set; }

        [JsonIgnore] // Bỏ qua khi serialize để tránh vòng lặp
        public List<UserAdminMessage> Messages { get; set; }
    }
}