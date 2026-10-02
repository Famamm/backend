namespace fleetops_backend.Application.Services
{
    public interface IDriverValidationService
    {
        void ValidateLicenseExpiry(DateTimeOffset? expiryDate);
    }
}