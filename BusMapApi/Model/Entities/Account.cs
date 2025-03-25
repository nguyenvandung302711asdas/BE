using BusMapApi.Models.Entities;

namespace BusMapApi.Model.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string NumberPhone { get; set; }
        public string Password { get; set; }

        // Navigation properties
        public List<FavoriteRoute> FavoriteRoutes { get; set; }
        public List<Chat> Chats { get; set; }
        public List<UserAdminChat> UserChats { get; set; }
        public List<UserAdminChat> AdminChats { get; set; }
        public List<GroupChatMessage> GroupChatMessages { get; set; }
    }
}