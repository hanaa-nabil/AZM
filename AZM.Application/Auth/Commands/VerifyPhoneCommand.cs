using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
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