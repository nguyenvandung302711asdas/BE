
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Admin_Bus.Models
//{
//    public class BaiVietQuangCao
//    {
//        [Key]
//        public int ID_BaiViet { get; set; }
//        public string TieuDe { get; set; }
//        public string NoiDung { get; set; }
//        public DateTime NgayDang { get; set; } = DateTime.Now;
//        public string TacGia { get; set; }

//        public List<AnhBaiViet> AnhBaiViets { get; set; } = new List<AnhBaiViet>();
//    }

//}
using BusMapApi.Model;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMapApi.Model.Entities
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

