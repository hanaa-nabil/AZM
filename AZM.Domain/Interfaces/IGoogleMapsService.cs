using AZM.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface IGoogleMapsService
    {
        Task<List<GeoLocation>> SnapToRoadsAsync(List<GeoLocation> waypoints);
        Task<string> GeocodeAsync(double lat, double lng);
        Task<int> EstimateRouteTimeAsync(List<GeoLocation> waypoints);
        Task<TrafficInfo> GetTrafficConditionAsync(GeoLocation from, GeoLocation to);
    }
}
