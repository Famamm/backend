namespace fleetops_backend.Application.Services
{
    public interface IFuelFillReader
    {
        Task<List<FuelFill>> GetLatestAsync(int vehicleId, int count);
    }
}
