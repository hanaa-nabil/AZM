using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Interfaces;
using Igor.Gateway.Dtos.Events;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AZM.Application.Events.Handlers
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Result<EventDto>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGoogleMapsService _maps;

        public CreateEventHandler(
            IEventRepository eventRepo,
            IUserRepository userRepo,
            IGoogleMapsService maps)
        {
            _eventRepo = eventRepo;
            _userRepo = userRepo;
            _maps = maps;
        }

        public async Task<Result<EventDto>> Handle(CreateEventCommand cmd, CancellationToken ct)
        {
            // 1. Snap the organizer-drawn waypoints to actual roads
            var snappedWaypoints = await _maps.SnapToRoadsAsync(cmd.Waypoints);

            // 2. Get human-readable address for the meeting point
            var address = await _maps.GeocodeAsync(cmd.MeetingLat, cmd.MeetingLng);

            // 3. Estimate how long the route takes to run
            var estimatedMinutes = await _maps.EstimateRouteTimeAsync(snappedWaypoints);

            // 4. Build the domain aggregate
            var runningEvent = RunningEvent.Create(
                organizerId: cmd.OrganizerId,
                title: cmd.Title,
                description: cmd.Description,
                scheduledAt: cmd.ScheduledAt,
                difficulty: cmd.Difficulty,
                maxParticipants: cmd.MaxParticipants,
                meetingPoint: new GeoLocation(cmd.MeetingLat, cmd.MeetingLng),
                meetingAddress: address,
                route: new GeoRoute(snappedWaypoints),
                estimatedMinutes: estimatedMinutes
            );

            await _eventRepo.AddAsync(runningEvent, ct);

            return Result<EventDto>.Success(EventDto.From(runningEvent));
        }
        public async Task<TrafficInfo> GetTrafficConditionAsync(GeoLocation from, GeoLocation to)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                      $"?origins={from.Lat},{from.Lng}" +
                      $"&destinations={to.Lat},{to.Lng}" +
                      $"&mode=driving"                   // must be driving for traffic data
              $"&departure_time={now}" +
              $"&traffic_model=best_guess" +
              $"&key={_apiKey}";

            var response = await _http.GetFromJsonAsync<DistanceMatrixResponse>(url);
            var element = response!.Rows[0].Elements[0];
            var normalSeconds = element.Duration.Value;
            var trafficSeconds = element.DurationInTraffic.Value;

            // More than 40% slower than normal = congested
            var isCongested = trafficSeconds > normalSeconds * 1.4;

            return new TrafficInfo
            {
                NormalMinutes = normalSeconds / 60,
                TrafficMinutes = trafficSeconds / 60,
                IsCongested = isCongested,
                DelayMinutes = (trafficSeconds - normalSeconds) / 60
            };
        }
    }
}
