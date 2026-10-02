using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Requests
{
    public class CreateSquadRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public List<SportType> Sports { get; set; } = new();
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationName { get; set; }
        public SquadPrivacy Privacy { get; set; } = SquadPrivacy.Public;
        public string? CoverImageUrl { get; set; }
    }
}
