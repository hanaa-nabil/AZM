using AZM.Application.Auth.DTOs;
using AZM.Application.Common;
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