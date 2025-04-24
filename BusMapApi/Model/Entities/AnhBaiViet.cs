using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusMapApi.Model.Entities
{
    public class AnhBaiViet
    {
        [Key]
        public int ID_Anh { get; set; }
        [ForeignKey("BaiVietQuangCao")]
        public int ID_BaiViet { get; set; }
        public string DuongDan { get; set; }  // Đường dẫn ảnh
        public string MoTa { get; set; }  // Mô tả ảnh
        [JsonIgnore]
        public BaiVietQuangCao? BaiVietQuangCao { get; set; }
    }
}
