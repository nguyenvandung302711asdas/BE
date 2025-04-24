using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMapApi.Model.Entities
{
    public class TripHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; } // Không cần ForeignKey ở đây vì đã khai báo bên dưới

        [Required]
        [StringLength(50)]
        public string BusNumber { get; set; }

        [Required]
        [StringLength(255)]
        public string StartLocation { get; set; }

        [Required]
        [StringLength(255)]
        public string EndLocation { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [NotMapped]
        public int? Duration => EndTime.HasValue ? (int)(EndTime.Value - StartTime).TotalMinutes : null;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Fare must be non-negative")]
        public decimal Fare { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Khóa ngoại đến bảng CustomerAccount
        [ForeignKey(nameof(CustomerId))] // Sửa lại tên đúng
        public virtual Account account { get; set; }
    }

}
