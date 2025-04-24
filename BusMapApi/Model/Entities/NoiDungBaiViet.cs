using BusMapApi.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusMapApi.Model.Entities{
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
