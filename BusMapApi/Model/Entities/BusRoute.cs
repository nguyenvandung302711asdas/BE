using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMapApi.Model.Entities
{
    public class BusRoute
    {
        public string Id { get; set; }
        public string RouteId { get; set; }
        public string Name { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }

        public ICollection<DetailBusRoute> DetailBusRoutes { get; set; }
    }


}
