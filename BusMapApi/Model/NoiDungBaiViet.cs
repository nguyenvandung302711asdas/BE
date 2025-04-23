using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Thêm namespace này

namespace BusMapApi.Model
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
        [JsonIgnore] // Thêm attribute này để tránh vòng lặp
        public BaiVietQuangCao? BaiVietQuangCao { get; set; }
    }
}