using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization; // Hoặc dùng Newtonsoft.Json nếu bạn cần

namespace BusMapApi.Model
{
    [Table("TravelInfo")] // 👈 Nếu bảng tên khác
    public class TravelInfo
    {
        public int TravelInfoId { get; set; }

        [Required]
        public string FromLocation { get; set; }

        [Required]
        public string ToLocation { get; set; }

        [Required]
        public DateTime TravelDateTime { get; set; }

        // Khóa ngoại đến User
        [Required]
        public int UserId { get; set; }

        // Ngăn không cho ASP.NET Core serialize/validate trường này
        [JsonIgnore]              // ✅ Nếu bạn dùng System.Text.Json (mặc định .NET 5+)
        [ValidateNever]           // ✅ Ngăn validation trong ASP.NET
        [NotMapped]               // ✅ Ngăn EF Core tự map nếu không cần thiết
        public User User { get; set; }
    }
}