using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminProfileMapper
    {
        AccountDTO ToAccountDto(User user);
    }
}
