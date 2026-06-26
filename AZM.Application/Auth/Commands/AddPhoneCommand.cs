using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class AddPhoneCommand : IRequest<Result<RegisterResponseDto>>
    {
        public AddPhoneRequestDto Dto { get; set; }

        public AddPhoneCommand(AddPhoneRequestDto dto)
        {
            Dto = dto;
        }
    }
}