using AZM.Domain.Entities;

namespace AZM.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneExistsAsync(string phoneNumber);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
        Task UpdateFcmTokenAsync(Guid userId, string fcmToken);
        Task ClearFcmTokenAsync(Guid userId, string fcmToken);
        Task<bool> UsernameExistsAsync(string username);
        Task RemoveUserSportsAsync(Guid userId);

        Task<User?> GetByIdWithDetailsAsync(Guid userId);
        Task UpdateAsync(User user);
        Task<bool> UpdateUsernameAsync(Guid userId, string newUsername);
        Task<List<UserProfile>> GetUsersWithStreakLastActiveAsync(DateOnly date);
        Task RecordDailyActivityAsync(Guid userId, DateOnly date);
        Task<List<UserDailyActivity>> GetRecentActivityAsync(Guid userId, int days);
    }
}