using AZM.Domain.Enums;

namespace AZM.Domain.Entities
{
    public class UserSport
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }      // ← change from string

        public Sport Sport { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}