using Microsoft.EntityFrameworkCore;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;

namespace fleetops_backend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuthValidationService _validator;
        private readonly IUsernameService _usernameService;

        public AuthService(
            AppDbContext context,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IAuthValidationService validator,
            IUsernameService usernameService)
        {
            _context = context;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _validator = validator;
            _usernameService = usernameService;
        }

        public async Task<LoginResponseDTO?> LoginAsync(string identity, string password)
        {
            if (!_validator.IsValidIdentity(identity) ||
                !_validator.IsValidPassword(password))
                return null;

            var user = await _context.Users
                .Include(u => u.Organization)
                .Include(u => u.Role)
                .Include(u => u.Driver)
                .FirstOrDefaultAsync(x => x.FullName == identity || x.Email == identity || x.Username == identity);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _refreshTokenService.CreateAsync(user.Id);

            var expiresInMinutes = _tokenService.GetAccessTokenExpiryMinutes();

            if (user.Role == null)
                throw new InvalidOperationException("Role tidak ditemukan.");

            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresInMinutes * 60,
                User = new UserInfoDTO
                {
                    UserId = user.Id,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.Organization?.OrganizationName ?? string.Empty,
                    DriverId = user.Driver?.Id,
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email ?? string.Empty,
                    RoleName = user.Role.Name
                }
            };
        }

        public async Task<bool?> RegisterAsync(string organizationName, string fullName, string email, string password)
        {
            ValidateRegisterInput(organizationName, fullName, email, password);

            if (await _context.Users.AnyAsync(x => x.Email == email))
                return null;

            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
                throw new InvalidOperationException("Role Admin belum ada.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var org = await CreateOrganizationAsync(organizationName);
                var user = await CreateAdminUserAsync(fullName, email, hashedPassword, org.Id, adminRole.Id, organizationName);

                user = await GetUserWithDetailsAsync(user.Id);

                if (user == null)
                    throw new InvalidOperationException("User tidak ditemukan setelah dibuat.");

                await transaction.CommitAsync();

                if (user.Role == null)
                    throw new InvalidOperationException("Role tidak ditemukan.");

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private void ValidateRegisterInput(string organizationName, string fullName, string email, string password)
        {
            if (!_validator.IsValidOrganizationName(organizationName))
                throw new InvalidOperationException("Nama organisasi tidak valid.");

            if (!_validator.IsValidFullName(fullName))
                throw new InvalidOperationException("Nama lengkap tidak valid.");

            if (!_validator.IsValidEmail(email))
                throw new InvalidOperationException("Email tidak valid.");

            if (!_validator.IsValidPassword(password))
                throw new InvalidOperationException("Password must be at least 8 characters.");
        }

        private async Task<Organization> CreateOrganizationAsync(string organizationName)
        {
            var org = new Organization
            {
                OrganizationName = organizationName,
                CreatedAt = DateTime.UtcNow
            };
            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();
            return org;
        }

        private async Task<User> CreateAdminUserAsync(
            string fullName,
            string email,
            string hashedPassword,
            int organizationId,
            int roleId,
            string organizationName)
        {
            var username = await _usernameService.GenerateUniqueUsernameAsync(fullName, organizationName);

            var user = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                OrganizationId = organizationId,
                RoleId = roleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<User?> GetUserWithDetailsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Organization)
                .Include(u => u.Role)
                .Include(u => u.Driver)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<RefreshTokenResponseDTO?> RefreshAccessTokenAsync(string refreshToken)
        {
            if (!_validator.IsValidRefreshToken(refreshToken))
                return null;

            var storedRefreshToken = await _refreshTokenService.FindValidAsync(refreshToken);

            if (storedRefreshToken == null)
                return null;

            var user = storedRefreshToken.User;
            if (user == null)
                return null;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var rotation = await _refreshTokenService.RotateAsync(storedRefreshToken, user.Id);
            await transaction.CommitAsync();

            var expiresInMinutes = _tokenService.GetAccessTokenExpiryMinutes();

            return new RefreshTokenResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = rotation.RawToken,
                ExpiresIn = expiresInMinutes * 60
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (!_validator.IsValidRefreshToken(refreshToken))
                return false;

            return await _refreshTokenService.RevokeAsync(refreshToken);
        }

        public async Task<bool> LogoutAllDevicesAsync(int userId)
        {
            return await _refreshTokenService.RevokeAllAsync(userId);
        }
    }
}