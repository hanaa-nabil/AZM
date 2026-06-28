namespace AZM.Api.Requests
{
    public record WaypointRequest(
        int Order,
        double Latitude,
        double Longitude
    );
}
