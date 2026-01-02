namespace Event_Management_System.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadImageFromBytesAsync(byte[] imageBytes, string fileName);
    }
}
