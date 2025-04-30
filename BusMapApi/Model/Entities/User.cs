using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BusMapApi.Model
{
    [Table("Users")]
    public class User
    {
            
        public int UserId { get; set; }
        public string Username { get; set; }
       // public string Password { get; set; } // nên mã hóa trong thực tế

        public ICollection<TravelInfo> TravelInfos { get; set; }
    }
}