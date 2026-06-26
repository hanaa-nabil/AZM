namespace AZM.Domain.ValueObjects
{
    public class GeoRoute
    {
        public List<GeoLocation> Points { get; set; } = new();

        public GeoRoute() { }

        public GeoRoute(List<GeoLocation> points)
        {
            Points = points;
        }
    }
}