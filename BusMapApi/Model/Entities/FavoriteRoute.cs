namespace BusMapApi.Model.Entities
{
    public class FavoriteRoute
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string RouteNo { get; set; }
        public string RouteName { get; set; }

        // Navigation property
        public Account User { get; set; }
    }
}