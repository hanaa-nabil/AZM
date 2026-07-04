using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
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