using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequestDto Dto) : IRequest<Result<AuthResponseDto>>;
}
