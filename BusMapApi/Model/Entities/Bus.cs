using System.ComponentModel.DataAnnotations;

namespace BusMapApi.Model.Entities
{
    public class Bus
    {
        [Key]  // Đánh dấu khóa chính
        public int Id { get; set; }
        public int RouteId { get; set; }
        public string RouteNo { get; set; }
        public string RouteName { get; set; }
        public string Address { get; set; }
    }
}
