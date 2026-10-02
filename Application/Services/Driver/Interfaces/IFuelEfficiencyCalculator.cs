using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IFuelEfficiencyCalculator
    {
        (DateTime StartOfCurrentMonth, DateTime StartOfLastMonth, DateTime EndOfLastMonth) GetMonthRanges(DateTime now);
        FuelEfficiencyResult Calculate(IEnumerable<Trip> vehicleTrips, IEnumerable<Fuel> vehicleFuels, DateTime startOfCurrentMonth, DateTime startOfLastMonth, DateTime endOfLastMonth);
    }
}

