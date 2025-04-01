namespace BusMapApi.Model.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string NumberPhone { get; set; }
        public required string Password { get; set; }
    }
}
