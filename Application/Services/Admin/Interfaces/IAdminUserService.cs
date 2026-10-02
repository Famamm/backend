using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminUserService
    {
        Task<(User user, string generatedPassword, string username)> CreateUserAsync(
            Organization organization,
            CreateUserDTO dto,
            string roleName,
            string? phoneNumber = null);

        Task<List<UserListItemDTO>> GetUsersAsync(int organizationId);
    }
}
