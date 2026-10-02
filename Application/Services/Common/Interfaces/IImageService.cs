using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image, string? folder = null);
        Task DeleteImageByUrlAsync(string imageUrl);
    }
}
