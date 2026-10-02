using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.DTOs.Admin;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class AdminProfileService : IAdminProfileService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        private readonly IAuthValidationService _validator;
        private readonly IAdminProfileMapper _mapper;

        public AdminProfileService(
            AppDbContext context,
            IImageService imageService,
            IAuthValidationService validator,
            IAdminProfileMapper mapper)
        {
            _context = context;
            _imageService = imageService;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<AccountDTO> GetAdminProfileAsync(int adminId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == adminId);

            if (user == null)
            {
                throw new KeyNotFoundException($"Admin dengan ID {adminId} tidak ditemukan");
            }

            return _mapper.ToAccountDto(user);
        }

        public async Task<UpdateAdminProfileDTO> UpdateAdminProfileAsync(int adminId, UpdateAdminProfileDTO dto)
        {
            var admin = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == adminId);

            if (admin == null)
                throw new KeyNotFoundException("Admin not found.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                if (!_validator.IsValidFullName(dto.FullName))
                    throw new InvalidOperationException("Nama lengkap tidak valid.");

                admin.FullName = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!_validator.IsValidEmail(dto.Email))
                    throw new InvalidOperationException("Email tidak valid.");

                admin.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                admin.Address = dto.Address.Trim();
            }

            if (dto.ProfilePhoto != null)
            {
                try
                {
                    var photoUrl = await _imageService.UploadImageAsync(dto.ProfilePhoto, "admin_profiles");
                    admin.ProfilePhoto = photoUrl;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upload photo: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return dto;
        }
        
    }
}
