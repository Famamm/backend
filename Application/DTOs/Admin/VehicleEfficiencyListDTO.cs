public class VehicleEfficiencyListDTO
{
    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = "";
    public string PlatNomor { get; set; } = "";
    public string VehicleType { get; set; } = "";
    public string FuelType { get; set; } = "";
    public int CurrentOdometer { get; set; }

    public double TotalJarak { get; set; }
    public double TotalBensin { get; set; }
    public double SkorEfisiensi { get; set; }
    public double VsBulanLalu { get; set; }

    public string StatusEfisiensi { get; set; } = "";
}