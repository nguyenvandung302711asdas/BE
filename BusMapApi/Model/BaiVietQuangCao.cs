using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model
{
    public class BaiVietQuangCao
    {
        [Key]
        public int ID_BaiViet { get; set; }
        public string TieuDe { get; set; }
        //public string NoiDung { get; set; }
        public DateTime NgayDang { get; set; } = DateTime.Now;
        public string TacGia { get; set; }

        public List<AnhBaiViet> AnhBaiViets { get; set; } = new List<AnhBaiViet>();
        public List<NoiDungBaiViet> NoiDungBaiViets { get; set; } = new List<NoiDungBaiViet>();

    }
}
