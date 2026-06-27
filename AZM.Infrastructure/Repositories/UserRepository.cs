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

    }
}