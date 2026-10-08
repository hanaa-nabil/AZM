using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{

    public interface ISquadPresence
    {
        void Connect(Guid squadId, Guid userId, string connectionId);
        void Disconnect(string connectionId);
        int OnlineCount(Guid squadId);
        bool IsOnline(Guid squadId, Guid userId);
    }
}
