using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAssignmentMapper
    {
        CurrentDriverDTO ToCurrentDriverDto(Vehicle vehicle);
        AvailableDriverDTO ToAvailableDriverDto(Driver driver);
    }
}
