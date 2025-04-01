namespace BusMapApi.Model.Entities
{
    public class DetailBusRoute
    {
       
            public string Id { get; set; }
            public string RouteId { get; set; }
            public string StopId { get; set; }
            public string StopName { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public BusRoute BusRoute { get; set; }
        

    }
}
