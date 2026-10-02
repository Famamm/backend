namespace fleetops_backend.Application.Services
{
    public enum DashboardRangeGranularity
    {
        Daily,
        Monthly
    }

    /// <summary>
    /// Rentang waktu (window) untuk chart dashboard beserta granularitas bucket-nya.
    /// Semua tanggal dihitung dalam zona waktu WIB (UTC+7).
    /// </summary>
    public sealed class DashboardRangeWindow
    {
        public const string Week = "week";
        public const string Month = "month";
        public const string Year = "year";

        public static readonly TimeSpan WibOffset = TimeSpan.FromHours(7);

        private const int WeekDays = 7;
        private const int MonthDays = 30;
        private const int YearMonths = 12;

        public string Range { get; }

        public DashboardRangeGranularity Granularity { get; }

        /// <summary>Tanggal WIB pertama pada window (inklusif).</summary>
        public DateOnly StartDate { get; }

        /// <summary>Tanggal WIB terakhir pada window (inklusif).</summary>
        public DateOnly EndDate { get; }

        /// <summary>Jumlah titik data (bucket) pada chart.</summary>
        public int BucketCount { get; }

        private DashboardRangeWindow(
            string range,
            DashboardRangeGranularity granularity,
            DateOnly startDate,
            DateOnly endDate,
            int bucketCount)
        {
            Range = range;
            Granularity = granularity;
            StartDate = startDate;
            EndDate = endDate;
            BucketCount = bucketCount;
        }

        /// <summary>
        /// Membuat window dari nilai query <paramref name="range"/>.
        /// Nilai yang didukung: <c>week</c> (default), <c>month</c>, <c>year</c>.
        /// </summary>
        public static DashboardRangeWindow Create(string? range, DateOnly today)
        {
            var normalized = string.IsNullOrWhiteSpace(range)
                ? Week
                : range.Trim().ToLowerInvariant();

            return normalized switch
            {
                Week => new DashboardRangeWindow(
                    Week,
                    DashboardRangeGranularity.Daily,
                    today.AddDays(-(WeekDays - 1)),
                    today,
                    WeekDays),
                Month => new DashboardRangeWindow(
                    Month,
                    DashboardRangeGranularity.Daily,
                    today.AddDays(-(MonthDays - 1)),
                    today,
                    MonthDays),
                Year => new DashboardRangeWindow(
                    Year,
                    DashboardRangeGranularity.Monthly,
                    new DateOnly(today.Year, today.Month, 1).AddMonths(-(YearMonths - 1)),
                    today,
                    YearMonths),
                _ => throw new ArgumentOutOfRangeException(
                    nameof(range),
                    "Range harus salah satu dari: week, month, year")
            };
        }

        /// <summary>Awal window dalam WIB (inklusif).</summary>
        public DateTimeOffset StartWib =>
            new(StartDate.ToDateTime(TimeOnly.MinValue), WibOffset);

        /// <summary>Akhir window dalam WIB (eksklusif).</summary>
        public DateTimeOffset EndWibExclusive =>
            new(EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue), WibOffset);

        /// <summary>Mengubah tanggal WIB menjadi key bucket sesuai granularitas.</summary>
        public DateOnly ToBucket(DateOnly date) =>
            Granularity == DashboardRangeGranularity.Monthly
                ? new DateOnly(date.Year, date.Month, 1)
                : date;

        /// <summary>Tanggal bucket ke-<paramref name="index"/> (0-based, dari terlama ke terbaru).</summary>
        public DateOnly BucketAt(int index) =>
            Granularity == DashboardRangeGranularity.Monthly
                ? StartDate.AddMonths(index)
                : StartDate.AddDays(index);
    }
}
