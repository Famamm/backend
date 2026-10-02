using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.Services
{
    public interface IFuelPhotoService
    {
        Task<(bool success, string? imageUrl, string? errorMessage)> UploadFuelPhotoAsync(IFormFile? fuelPhoto);
    }
}

