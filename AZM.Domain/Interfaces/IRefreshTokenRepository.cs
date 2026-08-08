using AZM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task RevokeAsync(string token);
        Task RevokeAllForUserAsync(Guid userId);
    }
}
