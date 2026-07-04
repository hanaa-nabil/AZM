using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public record ForgotPasswordCommand(ForgotPasswordRequestDto Dto) : IRequest<Result>;
}