using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.Handlers
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<object>>
    {
        private readonly UserManager<User> _userManager;

        public DeleteAccountCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<object>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                return Result<object>.Failure("Account not found.", 404);

            user.IsActive = false;
            user.DeletedAtUtc = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Result<object>.Failure(errors, 500);
            }

            return Result<object>.Success(new { }, 200);
        }
    }
}
