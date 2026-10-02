using fleetops_backend.Application.Services;

namespace fleetops_backend.Tests
{
    public class DashboardRangeWindowTests
    {
        private static readonly DateOnly Today = new(2026, 9, 25);

        [Fact]
        public void Create_WithNull_DefaultsToWeek()
        {
            var window = DashboardRangeWindow.Create(null, Today);

            Assert.Equal(DashboardRangeWindow.Week, window.Range);
            Assert.Equal(DashboardRangeGranularity.Daily, window.Granularity);
            Assert.Equal(7, window.BucketCount);
            Assert.Equal(new DateOnly(2026, 9, 19), window.StartDate);
            Assert.Equal(Today, window.EndDate);
        }

        [Fact]
        public void Create_WithMonth_ReturnsThirtyDailyBuckets()
        {
            var window = DashboardRangeWindow.Create("month", Today);

            Assert.Equal(DashboardRangeWindow.Month, window.Range);
            Assert.Equal(DashboardRangeGranularity.Daily, window.Granularity);
            Assert.Equal(30, window.BucketCount);
            Assert.Equal(new DateOnly(2026, 8, 27), window.StartDate);
            Assert.Equal(Today, window.EndDate);
        }

        [Fact]
        public void Create_WithYear_ReturnsTwelveMonthlyBuckets()
        {
            var window = DashboardRangeWindow.Create("year", Today);

            Assert.Equal(DashboardRangeWindow.Year, window.Range);
            Assert.Equal(DashboardRangeGranularity.Monthly, window.Granularity);
            Assert.Equal(12, window.BucketCount);
            Assert.Equal(new DateOnly(2025, 10, 1), window.StartDate);
            Assert.Equal(Today, window.EndDate);
        }

        [Fact]
        public void Create_IsCaseInsensitiveAndTrims()
        {
            var window = DashboardRangeWindow.Create("  YEAR ", Today);

            Assert.Equal(DashboardRangeWindow.Year, window.Range);
        }

        [Fact]
        public void Create_WithUnknownRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DashboardRangeWindow.Create("decade", Today));
        }

        [Fact]
        public void ToBucket_ForMonthly_GroupsByFirstDayOfMonth()
        {
            var window = DashboardRangeWindow.Create("year", Today);

            Assert.Equal(new DateOnly(2026, 9, 1), window.ToBucket(new DateOnly(2026, 9, 25)));
        }

        [Fact]
        public void ToBucket_ForDaily_KeepsDate()
        {
            var window = DashboardRangeWindow.Create("week", Today);

            Assert.Equal(new DateOnly(2026, 9, 25), window.ToBucket(new DateOnly(2026, 9, 25)));
        }

        [Fact]
        public void BucketAt_ForMonthly_AdvancesByMonth()
        {
            var window = DashboardRangeWindow.Create("year", Today);

            Assert.Equal(new DateOnly(2025, 10, 1), window.BucketAt(0));
            Assert.Equal(new DateOnly(2026, 9, 1), window.BucketAt(11));
        }

        [Fact]
        public void EndWibExclusive_IsDayAfterEndDate()
        {
            var window = DashboardRangeWindow.Create("week", Today);

            Assert.Equal(new DateOnly(2026, 9, 26), DateOnly.FromDateTime(window.EndWibExclusive.DateTime));
        }
    }
}
