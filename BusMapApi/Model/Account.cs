using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Or use Newtonsoft.Json if needed

namespace BusMapApi.Model
{
    public class Account
    {
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string NumberPhone { get; set; }
        
        [Required]
        [JsonIgnore] // Don't expose the password in the API response
        public string Password { get; set; } // Password should be hashed in practice
    }
}
