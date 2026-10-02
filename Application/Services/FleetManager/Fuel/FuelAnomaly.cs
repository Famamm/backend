namespace fleetops_backend.Application.Services
{
    public record FuelAnomaly(double KmPerLiter, double BaselineKmPerLiter)
    {
        public double Deviation => (KmPerLiter - BaselineKmPerLiter) / BaselineKmPerLiter;
    }
}
