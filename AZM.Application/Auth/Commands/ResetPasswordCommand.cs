using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public record ResetPasswordCommand(ResetPasswordRequestDto Dto) : IRequest<Result>;
}