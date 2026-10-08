using AZM.Domain.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Services
{
    public class SquadPresence : ISquadPresence
    {
        private readonly ConcurrentDictionary<string, (Guid SquadId, Guid UserId)> _connections = new();

        public void Connect(Guid squadId, Guid userId, string connectionId)
            => _connections[connectionId] = (squadId, userId);

        public void Disconnect(string connectionId) => _connections.TryRemove(connectionId, out _);

        public int OnlineCount(Guid squadId)
            => _connections.Values.Where(c => c.SquadId == squadId).Select(c => c.UserId).Distinct().Count();

        public bool IsOnline(Guid squadId, Guid userId)
            => _connections.Values.Any(c => c.SquadId == squadId && c.UserId == userId);
    }
}
