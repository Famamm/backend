using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminDriverService : IAdminDriverService
    {
        private readonly AppDbContext _context;
        private readonly IImageService _imageService;
        private readonly IAuthValidationService _validator;
        private readonly IDriverValidationService _driverValidator;
        private readonly IAdminDriverMapper _mapper;
        private readonly IAdminDriverValidator _adminValidator;
        private readonly IAdminUserService _userService;

        public AdminDriverService(
            AppDbContext context,
            IImageService imageService,
            IAuthValidationService validator,
            IDriverValidationService driverValidator,
            IAdminDriverMapper mapper,
            IAdminDriverValidator adminValidator,
            IAdminUserService userService)
        {
            _context = context;
            _imageService = imageService;
            _validator = validator;
            _driverValidator = driverValidator;
            _mapper = mapper;
            _adminValidator = adminValidator;
            _userService = userService;
        }

        public async Task<(Driver driver, string? generatedPassword, string? username)> CreateDriverAsync(int organizationId, CreateDriverDTO dto)
        {
            ValidateCreateDriverInput(organizationId, dto);

            var organization = await _context.Organizations.FindAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException("Organization not found.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var (user, generatedPassword, username) = await _userService.CreateUserAsync(organization, dto, "Driver");
                var driver = await CreateDriverRecordAsync(dto, user.Id, organization.Id);

                await transaction.CommitAsync();

                return (driver, generatedPassword, username);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Database save failed: " + dbEx.GetBaseException().Message, dbEx);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private void ValidateCreateDriverInput(int organizationId, CreateDriverDTO dto)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            if (!_validator.IsValidFullName(dto.FullName))
                throw new InvalidOperationException("FullName harus diisi.");
            
            _driverValidator.ValidateLicenseExpiry(dto.TanggalBerlaku);
        }

        private async Task<Driver> CreateDriverRecordAsync(
            CreateDriverDTO dto,
            int userId,
            int organizationId)
        {
            string? fotoLisensiUrl = null;
            if (dto.FotoLisensiFile != null)
                fotoLisensiUrl = await _imageService.UploadImageAsync(dto.FotoLisensiFile, "fleetops/licenses");

            var driver = _mapper.ToDriver(
                dto,
                userId,
                organizationId,
                fotoLisensiUrl);

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return driver;
        }

        public async Task<List<Driver>> GetDriversAsync(int organizationId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            return await _context.Drivers
                .Include(d => d.User)
                .Where(d => d.OrganizationId == organizationId && d.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<DriverDetailDTO> GetDriverByIdAsync(int organizationId, int driverId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var driver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d =>
                    d.Id == driverId &&
                    d.OrganizationId == organizationId &&
                    d.Status != "Deleted");

            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            var totalTrip = await _context.Trips.CountAsync(t =>t.DriverId == driverId &&
                                                            t.OrganizationId == organizationId &&
                                                            t.Status == "Finished");

            return _mapper.ToDriverDetailDto(driver, totalTrip);
        }

        public async Task<Driver> UpdateDriverAsync(int organizationId, int driverId, UpdateDriverDTO dto)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var driver = await _context.Drivers.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == driverId && d.Status != "Deleted");

            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            _adminValidator.ValidateDriverOrganization(driver, organizationId);

            _driverValidator.ValidateLicenseExpiry(dto.TanggalBerlaku);
            
            var user = driver.User ?? throw new InvalidOperationException("Associated user not found.");

            await ApplyUserUpdatesAsync(user, dto);
            await ApplyDriverUpdatesAsync(driver, dto);

            await _context.SaveChangesAsync();
            return driver;
        }

        private async Task ApplyUserUpdatesAsync(User user, UpdateDriverDTO dto)
        {
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

                var emailInUse = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != user.Id);
                if (emailInUse)
                    throw new InvalidOperationException("Email sudah digunakan oleh pengguna lain.");

                user.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Address))
            {
                user.Address = dto.Address;
            }

            if (dto.ProfilePhotoFile != null)
            {
                user.ProfilePhoto = await _imageService.UploadImageAsync(dto.ProfilePhotoFile, "fleetops/profiles");
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (!_validator.IsValidPassword(dto.Password))
                    throw new InvalidOperationException("Password must be at least 8 characters.");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }
        }

        private async Task ApplyDriverUpdatesAsync(Driver driver, UpdateDriverDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.JenisLisensi))
                driver.JenisLisensi = dto.JenisLisensi;

            if (dto.FotoLisensiFile != null)
                driver.FotoLisensi = await _imageService.UploadImageAsync(dto.FotoLisensiFile, "fleetops/licenses");

            if (dto.TanggalBerlaku.HasValue)
                driver.TanggalBerlaku = dto.TanggalBerlaku.Value.ToUniversalTime();

            if (!string.IsNullOrWhiteSpace(dto.Status))
                driver.Status = dto.Status;
        }

        public async Task DeleteDriverAsync(int organizationId, int driverId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var driver = await _context.Drivers.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == driverId && d.Status != "Deleted");
            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            _adminValidator.ValidateDriverOrganization(driver, organizationId);

            // Soft delete by marking status
            driver.Status = "Deleted";
            await _context.SaveChangesAsync();
        }

        public async Task SetDriverPasswordAsync(int organizationId, int driverId, SetDriverPasswordDTO dto)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var driver = await _context.Drivers.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == driverId && d.Status != "Deleted");
            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            _adminValidator.ValidateDriverOrganization(driver, organizationId);

            var user = driver.User ?? throw new InvalidOperationException("Associated user not found.");

            if (!_validator.IsValidPassword(dto.Password))
                throw new InvalidOperationException("Password must be at least 8 characters.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
        }

        public async Task<(Driver driver, string generatedPassword)> ResetDriverPasswordAsync(int organizationId, int driverId)
        {
            _adminValidator.ValidateOrganizationId(organizationId);

            var driver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            _adminValidator.ValidateDriverOrganization(driver, organizationId);

            var user = driver.User ?? throw new InvalidOperationException("Associated user not found.");

            var organization = await _context.Organizations.FindAsync(organizationId);
            if (organization == null)
                throw new KeyNotFoundException("Organization not found.");

            var generatedPassword = Common.PasswordGenerator.GenerateForUserOrg(
                user.FullName,
                organization.OrganizationName
            );

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            await _context.SaveChangesAsync();

            return (driver, generatedPassword);
        }
    }
}