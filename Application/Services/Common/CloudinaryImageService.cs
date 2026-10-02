using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace fleetops_backend.Application.Services
{
    public class CloudinaryImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                throw new InvalidOperationException("Cloudinary settings are not configured.");
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile image, string? folder = null)
        {
            folder ??= "fleetops/trips";

            await using var stream = image.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }

            return result.SecureUrl?.AbsoluteUri ?? result.Url?.AbsoluteUri
                ?? throw new InvalidOperationException("Cloudinary did not return an image URL.");
        }

        public async Task DeleteImageByUrlAsync(string imageUrl)
        {
            var publicId = GetPublicIdFromUrl(imageUrl);
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return;
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);
            if (result.Error != null && result.Result != "not found")
            {
                throw new InvalidOperationException(result.Error.Message);
            }
        }

        private static string? GetPublicIdFromUrl(string imageUrl)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
            {
                return null;
            }

            var path = uri.AbsolutePath.Trim('/');
            var segments = path.Split('/').ToList();
            if (segments.Count == 0)
            {
                return null;
            }

            if (segments[0].StartsWith('v') && segments[0].Length > 1 && int.TryParse(segments[0][1..], out _))
            {
                segments.RemoveAt(0);
            }

            var lastSegment = segments.Last();
            var fileName = Path.ChangeExtension(lastSegment, null);
            segments[segments.Count - 1] = fileName ?? lastSegment;

            return string.Join('/', segments);
        }
    }
}