using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class SocialCompleteRegistrationCommand : IRequest<Result<RegisterResponseDto>>
    {
        public SocialCompleteRegistrationDto Dto { get; set; }

        public SocialCompleteRegistrationCommand(SocialCompleteRegistrationDto dto)
        {
            Dto = dto;
        }
    }
}