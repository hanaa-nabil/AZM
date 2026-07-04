using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class ResendOtpCommand : IRequest<Result>
    {
        public ResendOtpRequestDto Dto { get; set; }

        public ResendOtpCommand(ResendOtpRequestDto dto)
        {
            Dto = dto;
        }
    }
}