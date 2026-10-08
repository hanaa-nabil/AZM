using AZM.Application.Common;
using AZM.Application.Users.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class SetLocationSharingHandler : IRequestHandler<SetLocationSharingCommand, Result<bool>>
    {
        private readonly IUserRepository _users;
        public SetLocationSharingHandler(IUserRepository users) => _users = users;

        public async Task<Result<bool>> Handle(SetLocationSharingCommand cmd, CancellationToken ct)
        {
            var user = await _users.GetByIdWithDetailsAsync(cmd.UserId);
            if (user?.Profile is null) return Result<bool>.Failure("Profile not found.", 404);

            user.Profile.SetLocationSharing(cmd.Enabled);
            await _users.UpdateAsync(user);
            return Result<bool>.Success(true);
        }
    }

}
