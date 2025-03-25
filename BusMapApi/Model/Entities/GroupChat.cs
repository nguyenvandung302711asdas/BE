namespace BusMapApi.Model.Entities
{
    public class GroupChat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }

        // Navigation property
        public List<GroupChatMessage> Messages { get; set; }
    }
    public class GroupChatMessage
    {
        public Guid Id { get; set; }
        public Guid GroupChatId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }

        // Navigation properties
        public GroupChat GroupChat { get; set; }
        public Account Sender { get; set; }
    }
}
