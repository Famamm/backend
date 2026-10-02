using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IFuelAnomalyService
    {
        Task CheckAsync(Vehicle vehicle, Fuel fuel);
    }
}
