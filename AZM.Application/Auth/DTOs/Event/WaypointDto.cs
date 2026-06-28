using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Event
{
    public record WaypointDto(int Order, double Latitude, double Longitude);

}
