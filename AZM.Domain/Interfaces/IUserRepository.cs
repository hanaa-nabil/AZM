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

        Task RemoveUserSportsAsync(Guid userId);
    }
}