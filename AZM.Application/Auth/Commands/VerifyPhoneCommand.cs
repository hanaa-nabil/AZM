using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class VerifyPhoneCommand : IRequest<Result>
    {
        public VerifyPhoneRequestDto Dto { get; set; }

        public VerifyPhoneCommand(VerifyPhoneRequestDto dto)
        {
            Dto = dto;
        }
    }
}