using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class GoogleSignInCommand : IRequest<Result<AuthResponseDto>>
    {
        public GoogleAuthRequestDto Dto { get; set; }

        public GoogleSignInCommand(GoogleAuthRequestDto dto)
        {
            Dto = dto;
        }
    }
}