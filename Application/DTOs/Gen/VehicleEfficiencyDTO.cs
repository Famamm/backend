namespace fleetops_backend.Application.DTOs
{
    public class VehicleEfficiencyDTO
    {
        public double TotalJarak { get; set; }
        public double TotalBensin { get; set; }
        public double SkorEfisiensi { get; set; }
        public double VsBulanLalu { get; set; }
        public string StatusEfisiensi { get; set; } = string.Empty;
    }
}