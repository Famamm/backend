using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFuelBaselineCalculator
    {
        FuelBaselineDTO Calculate(IReadOnlyList<FuelFill> fills);
    }
}
