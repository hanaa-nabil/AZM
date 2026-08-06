using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;
using System;

namespace AZM.Application.Auth.Commands
{
    public record LogoutCommand(Guid UserId, LogoutRequestDto Dto) : IRequest<Result>;
}
