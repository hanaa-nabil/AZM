namespace AZM.Domain.ValueObjects
{
    public class GeoLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public GeoLocation() { }

        public GeoLocation(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }
}