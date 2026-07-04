using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
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