using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class VerifyOtpCommand : IRequest<Result>
    {
        public VerifyOtpRequestDto Dto { get; set; }

        public VerifyOtpCommand(VerifyOtpRequestDto dto)
        {
            Dto = dto;
        }
    }
}