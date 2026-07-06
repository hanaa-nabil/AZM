using AZM.Application.Users.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class UploadProfilePhotoHandler : IRequestHandler<UploadProfilePhotoCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;

        public UploadProfilePhotoHandler(IUserRepository userRepository, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _photoService = photoService;
        }

        public async Task<string> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId.ToString())
                ?? throw new KeyNotFoundException("User not found.");

            // Convert IFormFile -> base64 string, since IPhotoService expects base64 + publicId
            string base64Image;
            using (var memoryStream = new MemoryStream())
            {
                await request.Photo.CopyToAsync(memoryStream, cancellationToken);
                var bytes = memoryStream.ToArray();
                var extension = Path.GetExtension(request.Photo.FileName).TrimStart('.').ToLowerInvariant();
                var mimeType = extension switch
                {
                    "png" => "image/png",
                    "jpg" or "jpeg" => "image/jpeg",
                    "webp" => "image/webp",
                    _ => "image/jpeg"
                };
                base64Image = $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
            }

            var publicId = $"profile_{request.UserId}";

            var url = await _photoService.UploadPhotoAsync(base64Image, publicId);

            user.ProfilePhotoUrl = url;
            await _userRepository.UpdateAsync(user);

            return url;
        }
    }
}
