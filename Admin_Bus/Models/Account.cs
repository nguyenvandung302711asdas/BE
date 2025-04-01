

namespace Admin_Bus.Models
{
    public class Account
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string NumberPhone { get; set; }
        public virtual ICollection<TripHistory> TripHistories { get; set; } = new List<TripHistory>();
        public List<UserAdminChat> UserChats { get; set; }
        public List<UserAdminChat> AdminChats { get; set; }
    }

}
