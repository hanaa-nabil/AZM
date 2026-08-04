using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AZM.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _db;

        public UserRepository(UserManager<User> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<bool> EmailExistsAsync(string email)
            => await _userManager.FindByEmailAsync(email) is not null;

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
            => await _userManager.Users
                .AnyAsync(u => u.PhoneNumber == phoneNumber);

        public async Task<User?> GetByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<User?> GetByIdAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<User?> GetByIdWithDetailsAsync(Guid userId)
            => await _db.Users
                .Include(u => u.Profile)
                .Include(u => u.Sports)
                .FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<bool> UsernameExistsAsync(string username)
            => await _userManager.FindByNameAsync(username) is not null;

        public async Task UpdateFcmTokenAsync(Guid userId, string fcmToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return;
            user.FcmToken = fcmToken;
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserSportsAsync(Guid userId)
        {
            var sports = await _db.UserSports
                .Where(s => s.UserId == userId)
                .ToListAsync();
            _db.UserSports.RemoveRange(sports);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateUsernameAsync(Guid userId, string newUsername)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return false;

            var result = await _userManager.SetUserNameAsync(user, newUsername);
            return result.Succeeded;
        }
        public async Task<List<UserProfile>> GetUsersWithStreakLastActiveAsync(DateOnly date)
        {
            return await _db.UserProfiles
                .Where(p => p.LastActiveDate == date && p.CurrentStreak > 0)
                .ToListAsync();
        }
        public async Task ClearFcmTokenAsync(Guid userId, string fcmToken)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
                return;

            // Only clear if the token matches what's stored — avoids wiping a
            // different device's active token if this user is logged in on multiple devices.
            if (user.FcmToken == fcmToken)
            {
                user.FcmToken = string.Empty;
                await _db.SaveChangesAsync();
            }
        }

    }
}