using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AZM.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
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
    }
}