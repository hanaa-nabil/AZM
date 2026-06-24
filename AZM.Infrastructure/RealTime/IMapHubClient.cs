using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.RealTime
{
    public interface IMapHubClient
    {
        Task ReceiveLocationUpdate(string userId, double lat, double lng);
        Task ReceiveSnapshot(Dictionary<string, (double lat, double lng)> positions);
        Task ReceiveSOS(string userId, double lat, double lng);
        Task RunnerFinished(string userId, TimeSpan finishTime);
    }
}
