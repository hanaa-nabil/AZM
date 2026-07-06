using AZM.Application.DTOs.User;
using MediatR;

namespace AZM.Application.Users.Commands
{
    public record UpdateProfileCommand(Guid UserId, UpdateProfileRequestDto Request) : IRequest<UserProfileDto>;
}
