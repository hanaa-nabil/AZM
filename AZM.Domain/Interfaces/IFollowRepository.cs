using AZM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface IFollowRepository
    {
        Task FollowAsync(Guid followerId, Guid followingId);
        Task UnfollowAsync(Guid followerId, Guid followingId);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
        Task<List<Guid>> GetFollowerIdsAsync(Guid userId);
        Task<List<User>> GetFollowersAsync(Guid userId);
        Task<List<User>> GetFollowingAsync(Guid userId);
        Task<int> GetFollowersCountAsync(Guid userId);
        Task<int> GetFollowingCountAsync(Guid userId);
    }
}
