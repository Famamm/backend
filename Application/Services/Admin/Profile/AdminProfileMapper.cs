using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminProfileMapper : IAdminProfileMapper
    {
        public AccountDTO ToAccountDto(User user)
        {
            return new AccountDTO
            {
                Information = new AccountInformationDTO
                {
                    PhotoUrl = user.ProfilePhoto,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Address = user.Address
                },
                UserName = user.Username,
                Role = user.Role?.Name ?? "Admin"
            };
        }
    }
}
