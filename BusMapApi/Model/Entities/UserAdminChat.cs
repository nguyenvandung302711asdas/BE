using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [JsonIgnore]
        public Account? User { get; set; } // Nullable

        [JsonIgnore]
        public Admin? Admin { get; set; } // Nullable

        [JsonIgnore]
        public List<UserAdminMessage> Messages { get; set; } = new List<UserAdminMessage>();
    }
}