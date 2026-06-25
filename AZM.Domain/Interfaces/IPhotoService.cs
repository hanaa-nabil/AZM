namespace AZM.Domain.Interfaces
{
    public interface IPhotoService
    {
        /// <summary>
        /// Uploads a base64-encoded image to Cloudinary and returns the secure URL.
        /// </summary>
        Task<string> UploadPhotoAsync(string base64Image, string publicId);
    }
}