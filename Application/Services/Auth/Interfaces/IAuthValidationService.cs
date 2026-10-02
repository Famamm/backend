namespace fleetops_backend.Application.Services
{
    public interface IAuthValidationService
    {
        bool IsValidPassword(string? password);
        bool IsValidEmail(string? email);
        bool IsValidFullName(string? fullName);
        bool IsValidPhoneNumber(string? phoneNumber);
        bool IsValidOrganizationName(string? organizationName);
        bool IsValidIdentity(string? identity);
        bool IsValidRefreshToken(string? refreshToken);

    }
}