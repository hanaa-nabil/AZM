using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public record ForgotPasswordCommand(ForgotPasswordRequestDto Dto) : IRequest<Result>;
}