namespace Event_Management_System.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadImageFromBytesAsync(byte[] imageBytes, string fileName);
    }
}
