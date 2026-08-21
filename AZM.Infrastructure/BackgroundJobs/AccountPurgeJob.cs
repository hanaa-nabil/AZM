using AZM.Domain.Entities;
using AZM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.BackgroundJobs
{
    public class AccountPurgeJob
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public AccountPurgeJob(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task RunAsync()
        {
            var cutoff = DateTime.UtcNow.AddDays(-30);

            var usersToDelete = await _context.Users
                .Where(u => !u.IsActive && u.DeletedAtUtc != null && u.DeletedAtUtc <= cutoff)
                .ToListAsync();

            foreach (var user in usersToDelete)
            {
                // Remove events they organized first — same reasoning as before:
                // Event->Organizer is Restrict, so this must happen before DeleteAsync.
                await _context.Events
                    .Where(e => e.OrganizerId == user.Id)
                    .ExecuteDeleteAsync();

                await _userManager.DeleteAsync(user);
            }
        }
    }
}
