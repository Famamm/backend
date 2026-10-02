using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerProfileService : IFleetManagerProfileService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        private readonly IAuthValidationService _validator;

        public FleetManagerProfileService(
            AppDbContext context,
            IImageService imageService,
            IAuthValidationService validator)
        {
            _context = context;
            _imageService = imageService;
            _validator = validator;
        }

        public async Task<AccountDTO> GetProfileAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"Fleet manager dengan ID {userId} tidak ditemukan");
            }

            return ToAccountDto(user);
        }

        public async Task<AccountDTO> UpdateProfileAsync(int userId, FleetManagerProfileDTO dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"Fleet manager dengan ID {userId} tidak ditemukan");
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                if (!_validator.IsValidFullName(dto.FullName))
                    throw new InvalidOperationException("Nama lengkap tidak valid.");

                user.FullName = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (!_validator.IsValidEmail(dto.Email))
                    throw new InvalidOperationException("Email tidak valid.");

                user.Email = dto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                if (!_validator.IsValidPhoneNumber(dto.PhoneNumber))
                    throw new InvalidOperationException("Nomor telepon tidak valid.");

                user.PhoneNumber = dto.PhoneNumber.Trim();
            }

            if (dto.ProfilePhoto != null)
            {
                var photoUrl = await _imageService.UploadImageAsync(dto.ProfilePhoto, "fleet_manager_profiles");
                user.ProfilePhoto = photoUrl;
            }

            await _context.SaveChangesAsync();

            return ToAccountDto(user);
        }

        private static AccountDTO ToAccountDto(User user)
        {
            return new AccountDTO
            {
                Information = new AccountInformationDTO
                {
                    PhotoUrl = user.ProfilePhoto,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Address = null,
                    PhoneNumber = user.PhoneNumber
                },
                UserName = user.Username,
                Role = user.Role?.Name ?? "FleetManager"
            };
        }
    }
}
