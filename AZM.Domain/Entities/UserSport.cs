using AZM.Domain.Enums;

namespace AZM.Domain.Entities
{
    public class UserSport
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Sport Sport { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}