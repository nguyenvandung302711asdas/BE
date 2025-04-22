using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace BusMapApi.Model
{
    public class TripHistory
    {
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public string StartLocation { get; set; }
        
        [Required]
        public string EndLocation { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; } // Nullable, not required
        
        [Required]
        public string RouteNumber { get; set; }
        
        public int? Cost { get; set; } // Nullable
        public int? DurationMinutes { get; set; } // Nullable
        public int? WalkingDistance { get; set; } // Nullable
        public float? BusDistance { get; set; } // Nullable

        // Navigational property, không ánh xạ vào cơ sở dữ liệu
        [JsonIgnore]              // ✅ Nếu bạn dùng System.Text.Json (mặc định .NET 5+)
        [ValidateNever]           // ✅ Ngăn validation trong ASP.NET
        [NotMapped]               // ✅ Ngăn EF Core tự map nếu không cần thiết
        public Account Customer { get; set; } // Navigational property
    }
}
