using AZM.Application.Auth.DTOs.Geo;
using AZM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly ILogger<GoogleMapsService> _logger;

        public GoogleMapsService(
            HttpClient http,
            IOptions<GoogleMapsOptions> options,
            ILogger<GoogleMapsService> logger)
        {
            _http = http;
            _apiKey = options.Value.ApiKey;
            _logger = logger;
        }

        public async Task<List<GeoLocation>> SnapToRoadsAsync(List<GeoLocation> waypoints)
        {
            // Roads API accepts max 100 points per request
            var path = string.Join("|", waypoints.Select(w => $"{w.Lat},{w.Lng}"));
            var url = $"https://roads.googleapis.com/v1/snapToRoads" +
                       $"?path={Uri.EscapeDataString(path)}&interpolate=true&key={_apiKey}";

            var response = await _http.GetFromJsonAsync<SnapToRoadsResponse>(url);

            if (response?.SnappedPoints is null or { Count: 0 })
            {
                _logger.LogWarning("SnapToRoads returned no points, falling back to original waypoints");
                return waypoints;
            }

            return response.SnappedPoints
                .Select(p => new GeoLocation(p.Location.Latitude, p.Location.Longitude))
                .ToList();
        }

        public async Task<string> GeocodeAsync(double lat, double lng)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json" +
                           $"?latlng={lat},{lng}&key={_apiKey}";
            var response = await _http.GetFromJsonAsync<GeocodeResponse>(url);

            return response?.Results.FirstOrDefault()?.FormattedAddress
                   ?? $"{lat:F4}, {lng:F4}";  // fallback to raw coords
        }

        public async Task<int> EstimateRouteTimeAsync(List<GeoLocation> waypoints)
        {
            var origin = $"{waypoints.First().Lat},{waypoints.First().Lng}";
            var destination = $"{waypoints.Last().Lat},{waypoints.Last().Lng}";
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                              $"?origins={origin}&destinations={destination}&mode=walking&key={_apiKey}";

            var response = await _http.GetFromJsonAsync<DistanceMatrixResponse>(url);
            var seconds = response?.Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Duration.Value ?? 0;

            return seconds / 60;
        }

        public async Task<TrafficInfo> GetTrafficConditionAsync(GeoLocation from, GeoLocation to)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                      $"?origins={from.Lat},{from.Lng}" +
                      $"&destinations={to.Lat},{to.Lng}" +
                      $"&mode=driving" +
                      $"&departure_time={now}" +
                      $"&traffic_model=best_guess" +
                      $"&key={_apiKey}";

            var response = await _http.GetFromJsonAsync<DistanceMatrixResponse>(url);
            var element = response?.Rows.FirstOrDefault()?.Elements.FirstOrDefault();

            if (element is null)
                return new TrafficInfo();

            var normal = element.Duration.Value;
            var traffic = element.DurationInTraffic?.Value ?? normal;

            return new TrafficInfo
            {
                NormalMinutes = normal / 60,
                TrafficMinutes = traffic / 60,
                IsCongested = traffic > normal * 1.4,
                DelayMinutes = Math.Max(0, (traffic - normal) / 60)
            };
        }

        public async Task<List<GeoRoute>> GetAlternativeRoutesAsync(GeoLocation origin, GeoLocation destination)
        {
            var url = "https://routes.googleapis.com/directions/v2:computeRoutes";
            var body = new
            {
                origin = new
                {
                    location = new { latLng = new { latitude = origin.Lat, longitude = origin.Lng } }
                },
                destination = new
                {
                    location = new { latLng = new { latitude = destination.Lat, longitude = destination.Lng } }
                },
                travelMode = "WALK",
                computeAlternativeRoutes = true
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask",
                "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");
            request.Content = JsonContent.Create(body);

            var httpResponse = await _http.SendAsync(request);
            httpResponse.EnsureSuccessStatusCode();

            var data = await httpResponse.Content.ReadFromJsonAsync<RoutesResponse>();

            return (data?.Routes ?? [])
                .Select(r => new GeoRoute(PolylineDecoder.Decode(r.Polyline.Value)))
                .ToList();
        }
    }
}
