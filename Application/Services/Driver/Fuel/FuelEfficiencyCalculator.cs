using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class FuelEfficiencyResult
    {
        public double TotalJarakCurrent { get; set; }
        public double TotalBensinCurrent { get; set; }
        public double SkorCurrent { get; set; }
        public double TotalJarakLast { get; set; }
        public double TotalBensinLast { get; set; }
        public double SkorLast { get; set; }
        public double VsBulanLalu { get; set; }
    }

    public class FuelEfficiencyCalculator : IFuelEfficiencyCalculator
    {
        public (DateTime StartOfCurrentMonth, DateTime StartOfLastMonth, DateTime EndOfLastMonth) GetMonthRanges(DateTime now)
        {
            var startOfCurrentMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfLastMonth = startOfCurrentMonth.AddMonths(-1);
            var endOfLastMonth = startOfCurrentMonth.AddTicks(-1);

            return (startOfCurrentMonth, startOfLastMonth, endOfLastMonth);
        }

        public FuelEfficiencyResult Calculate(
            IEnumerable<Trip> vehicleTrips,
            IEnumerable<Fuel> vehicleFuels,
            DateTime startOfCurrentMonth,
            DateTime startOfLastMonth,
            DateTime endOfLastMonth)
        {
            var startOfCurrentMonthOffset = new DateTimeOffset(startOfCurrentMonth);
            var startOfLastMonthOffset = new DateTimeOffset(startOfLastMonth);
            var endOfLastMonthOffset = new DateTimeOffset(endOfLastMonth);

            var tripsCurrent = vehicleTrips.Where(t => t.WaktuMulai >= startOfCurrentMonthOffset);
            var totalJarakCurrent = tripsCurrent.Sum(t => t.OdometerAkhir!.Value - t.OdometerAwal!.Value);

            var fuelsCurrent = vehicleFuels.Where(f => f.CreatedAt >= startOfCurrentMonth);
            var totalBensinCurrent = fuelsCurrent.Sum(f => f.JmlLiter);

            double skorCurrent = totalBensinCurrent > 0 ? (double)(totalJarakCurrent / totalBensinCurrent) : 0;

            var tripsLast = vehicleTrips.Where(t => t.WaktuMulai >= startOfLastMonthOffset && t.WaktuMulai <= endOfLastMonthOffset);
            var totalJarakLast = tripsLast.Sum(t => t.OdometerAkhir!.Value - t.OdometerAwal!.Value);

            var fuelsLast = vehicleFuels.Where(f => f.CreatedAt >= startOfLastMonth && f.CreatedAt <= endOfLastMonth);
            var totalBensinLast = fuelsLast.Sum(f => f.JmlLiter);

            double skorLast = totalBensinLast > 0 ? (double)(totalJarakLast / totalBensinLast) : 0;

            double vsBulanLalu = skorCurrent - skorLast;

            return new FuelEfficiencyResult
            {
                TotalJarakCurrent = totalJarakCurrent,
                TotalBensinCurrent = (double)totalBensinCurrent,
                SkorCurrent = skorCurrent,
                TotalJarakLast = totalJarakLast,
                TotalBensinLast = (double)totalBensinLast,
                SkorLast = skorLast,
                VsBulanLalu = vsBulanLalu
            };
        }
    }
}
