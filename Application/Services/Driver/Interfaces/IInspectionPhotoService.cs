using fleetops_backend.Models;
using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.Services
{
    public interface IInspectionPhotoService
    {
        Task<List<string>> UploadPhotosAsync(int inspectionId, List<IFormFile>? photos);
        Task DeleteOldPhotosAsync(ICollection<InspectionPhoto> oldPhotos);
        Task DeletePhotosAsync(ICollection<InspectionPhoto> photos);
        Task<int?> FindPhotoIdByUrlAsync(string imageUrl);
    }
}

