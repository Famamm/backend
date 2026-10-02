namespace fleetops_backend.Application.Services
{
    public interface IFuelAnomalyDetector
    {
        FuelAnomaly? Detect(IReadOnlyList<FuelFill> fills);
    }
}
