using MediatR;
using Microsoft.AspNetCore.Http;

namespace AZM.Application.Users.Commands
{
    public record UploadProfilePhotoCommand(Guid UserId, IFormFile Photo) : IRequest<string>;
}
