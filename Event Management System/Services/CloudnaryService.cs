using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Event_Management_System.Configuration;
using Event_Management_System.Interfaces;
using Microsoft.Extensions.Options;

namespace Event_Management_System.Services
{
    public class CloudnaryService : ICloudinaryService
    {
    
        public readonly Cloudinary _cloudinary;
        public CloudnaryService(IOptions<CloudinarySettings> cloudinary)
        {
            

            var account = new Account(
                cloudinary.Value.CloudName,
                cloudinary.Value.ApiKey,
                cloudinary.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result == null)
                throw new Exception("Upload returned null.");

            if (result.Error != null)
            {
                throw new Exception($"Cloudinary upload error: {result.Error.Message}");
            }

            // Cloudinary changed the property in newer versions to SecureUrl
            return result.SecureUrl?.ToString() ?? result.Url?.ToString();



        }

        public Task<string> UploadImageFromBytesAsync(byte[] imageBytes, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
