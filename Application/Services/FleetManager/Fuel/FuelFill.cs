namespace fleetops_backend.Application.Services
{
    public record FuelFill(int Odometer, decimal Liters)
    {
        public double? KmPerLiterSince(FuelFill previous)
        {
            var distance = Odometer - previous.Odometer;

            if (previous.Odometer <= 0 || distance <= 0 || Liters <= 0)
            {
                return null;
            }

            return distance / (double)Liters;
        }
    }
}
