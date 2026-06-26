using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public record LoginCommand(LoginRequestDto Dto) : IRequest<Result<AuthResponseDto>>;
}