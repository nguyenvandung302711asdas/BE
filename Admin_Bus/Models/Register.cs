using Microsoft.AspNetCore.Mvc;

namespace Admin_Bus.Models
{
    public class Register
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string NumberPhone { get; set; }
    }
}
