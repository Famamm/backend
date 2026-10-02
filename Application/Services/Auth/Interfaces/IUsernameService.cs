namespace fleetops_backend.Application.Services
{
    public interface IUsernameService
    {
        Task<string> GenerateUniqueUsernameAsync(
            string fullName,
            string organizationName);
    }
}