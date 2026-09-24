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
    public class UpdateActivityStatsHandler : IRequestHandler<UpdateActivityStatsCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;

        public UpdateActivityStatsHandler(IUserRepository userRepository) => _userRepository = userRepository;

        public async Task<Result<bool>> Handle(UpdateActivityStatsCommand request, CancellationToken ct)
        {
            if (request.Steps < 0 || request.DistanceMeters < 0)
                return Result<bool>.Failure("Steps and distance must be non-negative.");

            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId);
            if (user?.Profile is null)
                return Result<bool>.Failure("User profile not found.", 404);

            user.Profile.TotalStepCount += request.Steps;
            user.Profile.TotalDistanceMeters += request.DistanceMeters;

            if (request.Steps > 0 || request.DistanceMeters > 0)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                user.Profile.RegisterActivity(today);
                await _userRepository.RecordDailyActivityAsync(request.UserId, today);
            }

            await _userRepository.UpdateAsync(user);
            return Result<bool>.Success(true);
        }
    }
}
