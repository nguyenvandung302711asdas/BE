using Admin_Bus.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Admin_Bus.Models
{
    public class NoiDungBaiViet
    {
        [Key]
        public int ID_NoiDung { get; set; }

        [ForeignKey("BaiVietQuangCao")]
        public int ID_BaiViet { get; set; }

        public int ThuTu { get; set; } // Thứ tự hiển thị nội dung

        [Required]
        public string NoiDung { get; set; } // Nội dung bài viết

        // Navigation Property

        public BaiVietQuangCao? BaiVietQuangCao { get; set; }
    }
}
