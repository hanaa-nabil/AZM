using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Squad
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Bio { get; private set; }
        public SportType PrimarySport { get; private set; }
        public List<SportType> Sports { get; private set; } = new();
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public string? LocationName { get; private set; }
        public SquadPrivacy Privacy { get; private set; }
        public string? CoverImageUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Guid FounderId { get; private set; }
        public User Founder { get; private set; } = null!;
        public Guid? PinnedEventId { get; private set; }
        public void PinEvent(Guid? eventId) => PinnedEventId = eventId;
        public ICollection<SquadMember> Members { get; private set; } = new List<SquadMember>();

        public int MemberCount => Members.Count(m => m.Status == SquadMemberStatus.Approved);

        private Squad() { }

        public static Squad Create(
            string name, string? bio, List<SportType> sports,
            double? latitude, double? longitude, string? locationName,
            SquadPrivacy privacy, string? coverImageUrl, Guid founderId)
        {
            if (sports is null || sports.Count == 0)
                throw new ArgumentException("A squad must have at least one sport.");

            return new Squad
            {
                Id = Guid.NewGuid(),
                Name = name,
                Bio = bio,
                Sports = sports,
                PrimarySport = sports[0],
                Latitude = latitude,
                Longitude = longitude,
                LocationName = locationName,
                Privacy = privacy,
                CoverImageUrl = coverImageUrl,
                FounderId = founderId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
