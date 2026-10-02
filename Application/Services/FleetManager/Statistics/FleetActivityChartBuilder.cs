using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public static class FleetActivityChartBuilder
    {
        /// <summary>
        /// Membangun chart harian untuk <paramref name="windowDays"/> hari terakhir
        /// (berakhir pada <paramref name="today"/>).
        /// </summary>
        public static List<FleetActivityDayDTO> Build(DateOnly today, int windowDays, IEnumerable<DateOnly> tripDates)
            => BuildDaily(today.AddDays(-(windowDays - 1)), windowDays, tripDates);

        /// <summary>Membangun chart harian mulai <paramref name="startDate"/> selama <paramref name="days"/> hari.</summary>
        public static List<FleetActivityDayDTO> BuildDaily(DateOnly startDate, int days, IEnumerable<DateOnly> tripDates)
        {
            var counts = tripDates
                .GroupBy(d => d)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<FleetActivityDayDTO>(days);

            for (var index = 0; index < days; index++)
            {
                var date = startDate.AddDays(index);
                result.Add(new FleetActivityDayDTO
                {
                    Tanggal = date,
                    JumlahPerjalanan = counts.TryGetValue(date, out var count) ? count : 0
                });
            }

            return result;
        }

        /// <summary>
        /// Membangun chart bulanan mulai <paramref name="startMonth"/> selama <paramref name="months"/> bulan.
        /// Setiap titik memakai tanggal hari pertama bulan tersebut.
        /// </summary>
        public static List<FleetActivityDayDTO> BuildMonthly(DateOnly startMonth, int months, IEnumerable<DateOnly> tripDates)
        {
            var counts = tripDates
                .GroupBy(d => new DateOnly(d.Year, d.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<FleetActivityDayDTO>(months);

            for (var index = 0; index < months; index++)
            {
                var month = startMonth.AddMonths(index);
                result.Add(new FleetActivityDayDTO
                {
                    Tanggal = month,
                    JumlahPerjalanan = counts.TryGetValue(month, out var count) ? count : 0
                });
            }

            return result;
        }
    }
}
