using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminFleetManagerService : IAdminFleetManagerService
    {
        private const string FleetManagerRole = "FleetManager";

        private readonly AppDbContext _context;
        private readonly IAdminUserService _userService;
        private readonly IAdminDriverValidator _adminValidator;
        private readonly IImageService _imageService;
        private readonly IAuthValidationService _validator;

        public AdminFleetManagerService(
            AppDbContext context,
            IAdminUserService userService,
            IAdminDriverValidator adminValidator,
            IImageService imageService,
            IAuthValidationService validator)
        {
            _context = context;
            _userService = userService;
            _adminValidator = adminValidator;
            _imageService = imageService;
            _validator = validator;
        }

        public async Task<(User user, string generatedPassword, string username)> CreateFleetManagerAsync( int organizationId,
            CreateFleetManagerDTO dto)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var organization = await _context.Organizations.FindAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException("Organization not found.");

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && !_validator.IsValidPhoneNumber(dto.PhoneNumber))
                throw new InvalidOperationException("Nomor telepon tidak valid.");

            var result = await _userService.CreateUserAsync(organization, dto, FleetManagerRole, dto.PhoneNumber);

            _context.FleetManagers.Add(new FleetManager
            {
                UserId = result.user.Id,
                OrganizationId = organizationId,
                Status = "Active"
            });
            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<List<UserListItemDTO>> GetFleetManagersAsync(int organizationId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.FleetManager)
                .Where(u => u.OrganizationId == organizationId && u.Role!.Name == FleetManagerRole)
                .OrderBy(u => u.FullName)
                .Select(u => new UserListItemDTO
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Username = u.Username,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePhoto = u.ProfilePhoto,
                    RoleName = FleetManagerRole,
                    Status = u.FleetManager != null ? u.FleetManager.Status : null
                })
                .ToListAsync();
        }

        public async Task<FleetManagerDetailDTO> GetFleetManagerByIdAsync(int organizationId, int userId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var user = await GetFleetManagerOrThrowAsync(organizationId, userId);

            return new FleetManagerDetailDTO
            {
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                ProfilePhoto = user.ProfilePhoto,
                RoleName = FleetManagerRole,
                Status = user.FleetManager?.Status
            };
        }

        public async Task<User> UpdateFleetManagerAsync(int organizationId, int userId, UpdateFleetManagerDTO dto)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var user = await GetFleetManagerOrThrowAsync(organizationId, userId);

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                if (!_validator.IsValidFullName(dto.FullName))
                    throw new InvalidOperationException("Nama lengkap tidak valid.");

                user.FullName = dto.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.Address = dto.Address.Trim();

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                if (!_validator.IsValidPhoneNumber(dto.PhoneNumber))
                    throw new InvalidOperationException("Nomor telepon tidak valid.");

                user.PhoneNumber = dto.PhoneNumber.Trim();
            }

            if (dto.ProfilePhotoFile != null)
                user.ProfilePhoto = await _imageService.UploadImageAsync(dto.ProfilePhotoFile, "fleetops/profiles");

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (user.FleetManager == null)
                {
                    user.FleetManager = new FleetManager
                    {
                        UserId = user.Id,
                        OrganizationId = organizationId,
                        Status = dto.Status.Trim()
                    };
                }
                else
                {
                    user.FleetManager.Status = dto.Status.Trim();
                }
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteFleetManagerAsync(int organizationId, int userId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var user = await GetFleetManagerOrThrowAsync(organizationId, userId);

            // User tidak punya kolom status, jadi fleet manager dihapus permanen.
            // RefreshToken & Notification ikut terhapus lewat cascade delete.
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(User user, string generatedPassword)> ResetFleetManagerPasswordAsync(int organizationId, int userId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var user = await GetFleetManagerOrThrowAsync(organizationId, userId);

            var organization = await _context.Organizations.FindAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException("Organization not found.");

            var generatedPassword = Common.PasswordGenerator.GenerateForUserOrg(
                user.FullName,
                organization.OrganizationName);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            await _context.SaveChangesAsync();

            return (user, generatedPassword);
        }

        private async Task<User> GetFleetManagerOrThrowAsync(int organizationId, int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.FleetManager)
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.OrganizationId == organizationId &&
                    u.Role!.Name == FleetManagerRole);

            if (user == null)
                throw new KeyNotFoundException("Fleet manager not found.");

            return user;
        }
    }
}
