using BusMapApi.Model.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMapApi.Models.Entities
{
    [Table("Chat")]
    public class Chat
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Account User { get; set; }
        public List<DetailChat> DetailChats { get; set; }
    }

    [Table("DetailChat")]
    public class DetailChat
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ChatId { get; set; }

        // Navigation property
        public Chat Chat { get; set; }
    }

}