using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.RealTime
{
    public class LocationBroadcaster
    {
        private readonly IHubContext<MapHub, IMapHubClient> _hub;

        public LocationBroadcaster(IHubContext<MapHub, IMapHubClient> hub)
        {
            _hub = hub;
        }

        public async Task BroadcastLocationAsync(string sessionId, string userId, double lat, double lng)
        {
            await _hub.Clients.Group(sessionId)
                .ReceiveLocationUpdate(userId, lat, lng);
        }

        public async Task BroadcastFinishAsync(string sessionId, string userId, TimeSpan finishTime)
        {
            await _hub.Clients.Group(sessionId)
                .RunnerFinished(userId, finishTime);
        }
    }
}
