using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly AppDbContext _context;

        public FollowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task FollowAsync(Guid followerId, Guid followingId)
        {
            var exists = await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
            if (exists)
                return;

            _context.Follows.Add(new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId
            });
            await _context.SaveChangesAsync();
        }

        public async Task UnfollowAsync(Guid followerId, Guid followingId)
        {
            await _context.Follows
                .Where(f => f.FollowerId == followerId && f.FollowingId == followingId)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<List<Guid>> GetFollowerIdsAsync(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Select(f => f.FollowerId)
                .ToListAsync();
        }

        public async Task<List<User>> GetFollowersAsync(Guid userId)
        {
            var followerIds = await GetFollowerIdsAsync(userId);
            return await _context.Users
                .Where(u => followerIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<User>> GetFollowingAsync(Guid userId)
        {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingId)
                .ToListAsync();

            return await _context.Users
                .Where(u => followingIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<int> GetFollowersCountAsync(Guid userId)
        {
            return await _context.Follows.CountAsync(f => f.FollowingId == userId);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await _context.Follows.CountAsync(f => f.FollowerId == userId);
        }
    }
}
