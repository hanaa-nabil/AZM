using AZM.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AZM.Infrastructure.RealTime
{
    public class MapHub : Hub<IMapHubClient>  
    {
        private readonly ILocationCacheService _cache;

        public MapHub(ILocationCacheService cache)
        {
            _cache = cache;
        }

        // Runner joins the event group
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);

            // Send snapshot of all current positions to the late joiner
            var positions = await _cache.GetAllLocationsAsync(sessionId);
            await Clients.Caller.ReceiveSnapshot(positions);
        }

        // Runner leaves the event group
        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }

        // Runner sends their GPS position
        public async Task UpdateLocation(string sessionId, double lat, double lng)
        {
            var userId = Context.UserIdentifier!;

            // Save to Redis
            await _cache.SetLocationAsync(sessionId, userId, lat, lng);

            // Broadcast to everyone else in the group
            await Clients.OthersInGroup(sessionId)
                .ReceiveLocationUpdate(userId, lat, lng);
        }

        // Runner triggers SOS
        public async Task TriggerSOS(string sessionId, double lat, double lng)
        {
            var userId = Context.UserIdentifier!;
            await Clients.Group(sessionId).ReceiveSOS(userId, lat, lng);
        }
    }
}
