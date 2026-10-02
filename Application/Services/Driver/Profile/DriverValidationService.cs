namespace fleetops_backend.Application.Services
{
    public class DriverValidationService : IDriverValidationService
    {
        public void ValidateLicenseExpiry(DateTimeOffset? expiryDate)
        {
            if (!expiryDate.HasValue)
                throw new InvalidOperationException("Tanggal berlaku lisensi harus diisi.");

            if (expiryDate.Value <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException("Lisensi sudah expired.");
        }
    }
}