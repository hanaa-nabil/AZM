using AZM.Domain.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace AZM.Infrastructure.Services
{
    public class CloudinaryPhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryPhotoService(IOptions<CloudinarySettings> options)
        {
            var s = options.Value;
            var account = new Account(s.CloudName, s.ApiKey, s.ApiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadPhotoAsync(string base64Image, string publicId)
        {
            // Strip the data URI prefix if present (e.g. "data:image/jpeg;base64,")
            var base64Data = base64Image.Contains(',')
                ? base64Image.Split(',')[1]
                : base64Image;

            var bytes = Convert.FromBase64String(base64Data);

            using var stream = new MemoryStream(bytes);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(publicId, stream),
                PublicId = $"profile_photos/{publicId}",
                Overwrite = true,
                Transformation = new Transformation()
                    .Width(400).Height(400)
                    .Crop("fill")
                    .Gravity("face")   // centres crop on the face if detected
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                throw new InvalidOperationException(
                    $"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }
    }
}