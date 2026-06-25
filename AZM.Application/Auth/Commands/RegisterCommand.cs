using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class RegisterCommand : IRequest<Result<RegisterResponseDto>>
    {
        public RegisterRequestDto Dto { get; set; }

        public RegisterCommand(RegisterRequestDto dto)
        {
            Dto = dto;
        }
    }
}