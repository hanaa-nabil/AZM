using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class VerifyOtpCommand : IRequest<Result<VerifyOtpResponseDto>>
    {
        public VerifyOtpRequestDto Dto { get; set; }
        public VerifyOtpCommand(VerifyOtpRequestDto dto) => Dto = dto;
    }
}