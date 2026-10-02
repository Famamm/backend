using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public static class DriverStatisticsCalculator
    {
        private const double SecondsPerHour = 3600;
        private const int Decimals = 2;

        public static DriverStatisticsDto Calculate(int totalTrip, double totalJarak, long totalWaktuSeconds)
        {
            var totalJam = totalWaktuSeconds / SecondsPerHour;

            return new DriverStatisticsDto
            {
                TotalTrip = totalTrip,
                TotalJarak = totalJarak,
                TotalJamMengemudi = Math.Round(totalJam, Decimals),
                RataRataKecepatan = totalJam > 0 ? Math.Round(totalJarak / totalJam, Decimals) : 0
            };
        }
    }
}
