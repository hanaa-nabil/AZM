using AZM.Application.Auth.DTOs;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class CompleteProfileCommand : IRequest<Result<AuthResponseDto>>
    {
        public CompleteProfileRequestDto Dto { get; set; }

        public CompleteProfileCommand(CompleteProfileRequestDto dto)
        {
            Dto = dto;
        }
    }
}