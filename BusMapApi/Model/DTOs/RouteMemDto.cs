using System.Xml.Serialization;

namespace BusMapApi.Model.DTOs
{
    [XmlRoot("Route_Mem")]
    public class RouteMemDto
    {
        [XmlElement("RouteId")]
        public int RouteId { get; set; }

        [XmlElement("RouteName")]
        public string RouteName { get; set; }

        [XmlElement("RouteNo")]
        public string RouteNo { get; set; }
    }
}