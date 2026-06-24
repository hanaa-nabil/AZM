using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Auth.Commands
{
    public class SocialCompleteRegistrationCommand : IRequest<Result<AuthResponseDto>>
    {
        public SocialCompleteRegistrationDto Dto { get; set; }

        public SocialCompleteRegistrationCommand(SocialCompleteRegistrationDto dto)
        {
            Dto = dto;
        }
    }
}