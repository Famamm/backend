using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        private readonly IAuthValidationService _validator;
        private readonly IUsernameService _usernameService;
        private readonly IAdminDriverValidator _adminValidator;

        public AdminUserService(
            AppDbContext context,
            IImageService imageService,
            IAuthValidationService validator,
            IUsernameService usernameService,
            IAdminDriverValidator adminValidator)
        {
            _context = context;
            _imageService = imageService;
            _validator = validator;
            _usernameService = usernameService;
            _adminValidator = adminValidator;
        }

        public async Task<(User user, string generatedPassword, string username)> CreateUserAsync(
            Organization organization,
            CreateUserDTO dto,
            string roleName,
            string? phoneNumber = null)
        {
            if (!_validator.IsValidFullName(dto.FullName))
                throw new InvalidOperationException("FullName harus diisi.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
                throw new InvalidOperationException($"Role {roleName} belum ada.");

            var generatedPassword = Common.PasswordGenerator.GenerateForUserOrg(dto.FullName, organization.OrganizationName);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            var username = await _usernameService.GenerateUniqueUsernameAsync(dto.FullName, organization.OrganizationName);

            string? profilePhotoUrl = null;
            if (dto.ProfilePhotoFile != null)
                profilePhotoUrl = await _imageService.UploadImageAsync(dto.ProfilePhotoFile, "fleetops/profiles");

            var user = new User
            {
                FullName = dto.FullName,
                Username = username,
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address,
                PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
                ProfilePhoto = profilePhotoUrl,
                PasswordHash = hashedPassword,
                OrganizationId = organization.Id,
                RoleId = role.Id,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (user, generatedPassword, username);
        }

        public async Task<List<UserListItemDTO>> GetUsersAsync(int organizationId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Driver)
                .Include(u => u.FleetManager)
                .Where(u => u.OrganizationId == organizationId && u.Role!.Name != "Admin")
                .OrderBy(u => u.FullName)
                .ToListAsync();

            // Driver yang sudah di-soft-delete tidak ikut ditampilkan, mengikuti
            // perilaku GET /api/admin/drivers yang menyaring Status "Deleted".
            return users
                .Where(u => u.Driver == null || u.Driver.Status != "Deleted")
                .Select(u => new UserListItemDTO
                {
                    UserId = u.Id,
                    DriverId = u.Driver?.Id,
                    FullName = u.FullName,
                    Username = u.Username,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePhoto = u.ProfilePhoto,
                    RoleName = u.Role?.Name ?? string.Empty,
                    Status = u.Driver?.Status ?? u.FleetManager?.Status
                })
                .ToList();
        }
    }
}
