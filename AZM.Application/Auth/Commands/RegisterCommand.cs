using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
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