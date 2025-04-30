using BusMapApi.Model;

namespace BusMapApi.Model.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string NumberPhone { get; set; }
        public required string Password { get; set; }
  
        // Navigation properties
        public List<UserAdminChat> UserChats { get; set; }

        public List<FavoriteRoute> FavoriteRoutes { get; set; }
        public List<Chat> Chats { get; set; }
        public List<GroupChatMessage> GroupChatMessages { get; set; }



    }
}
