using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.Services
{
    public class FuelPhotoService : IFuelPhotoService
    {
        private readonly IImageService _imageService;

        public FuelPhotoService(IImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<(bool success, string? imageUrl, string? errorMessage)> UploadFuelPhotoAsync(IFormFile? fuelPhoto)
        {
            if (fuelPhoto == null)
            {
                return (true, null, null);
            }

            try
            {
                var imageUrl = await _imageService.UploadImageAsync(fuelPhoto, "fleetops/fuels");
                return (true, imageUrl, null);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Gagal mengupload foto BBM: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                return (false, null, $"Gagal upload foto: {ex.Message}");
            }
        }
    }
}
