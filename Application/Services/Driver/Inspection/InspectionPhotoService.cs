using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class InspectionPhotoService : IInspectionPhotoService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;

        public InspectionPhotoService(AppDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public async Task<List<string>> UploadPhotosAsync(int inspectionId, List<IFormFile>? photos)
        {
            var photoUrls = new List<string>();

            if (photos != null && photos.Count > 0)
            {
                foreach (var photo in photos)
                {
                    if (photo.Length > 0)
                    {
                        var imageUrl = await _imageService.UploadImageAsync(photo);
                        photoUrls.Add(imageUrl);

                        var inspectionPhoto = new InspectionPhoto
                        {
                            InspectionId = inspectionId,
                            ImageUrlPath = imageUrl,
                            IsRequired = true
                        };

                        _context.Set<InspectionPhoto>().Add(inspectionPhoto);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return photoUrls;
        }

        public async Task DeleteOldPhotosAsync(ICollection<InspectionPhoto> oldPhotos)
        {
            if (oldPhotos != null && oldPhotos.Count > 0)
            {
                foreach (var oldPhoto in oldPhotos.ToList())
                {
                    if (!string.IsNullOrWhiteSpace(oldPhoto.ImageUrlPath))
                    {
                        try
                        {
                            await _imageService.DeleteImageByUrlAsync(oldPhoto.ImageUrlPath);
                        }
                        catch
                        {
                            // ignore Cloudinary delete failure for now, but keep DB cleanup consistent
                        }
                    }
                    _context.Set<InspectionPhoto>().Remove(oldPhoto);
                }
            }
        }

        public async Task DeletePhotosAsync(ICollection<InspectionPhoto> photos)
        {
            if (photos != null && photos.Count > 0)
            {
                foreach (var photo in photos)
                {
                    if (!string.IsNullOrWhiteSpace(photo.ImageUrlPath))
                    {
                        try
                        {
                            await _imageService.DeleteImageByUrlAsync(photo.ImageUrlPath);
                        }
                        catch
                        {
                            // ignore Cloudinary delete failure and continue database cleanup
                        }
                    }
                }
            }

            if (photos != null && photos.Count > 0)
            {
                _context.Set<InspectionPhoto>().RemoveRange(photos);
            }
        }

        public async Task<int?> FindPhotoIdByUrlAsync(string imageUrl)
        {
            var photo = await _context.Set<InspectionPhoto>()
                .FirstOrDefaultAsync(p => p.ImageUrlPath == imageUrl);
            return photo?.Id;
        }
    }
}
