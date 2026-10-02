using fleetops_backend.Application.Services;

namespace fleetops_backend.Tests
{
    public class FleetActivityChartBuilderTests
    {
        [Fact]
        public void Build_WithNoTrips_ReturnsWindowWithZeroCounts()
        {
            var today = new DateOnly(2026, 9, 30);

            var result = FleetActivityChartBuilder.Build(today, 7, Array.Empty<DateOnly>());

            Assert.Equal(7, result.Count);
            Assert.All(result, day => Assert.Equal(0, day.JumlahPerjalanan));
            Assert.Equal(new DateOnly(2026, 9, 24), result.First().Tanggal);
            Assert.Equal(today, result.Last().Tanggal);
        }

        [Fact]
        public void Build_CountsTripsPerDay()
        {
            var today = new DateOnly(2026, 9, 30);
            var tripDates = new[]
            {
                new DateOnly(2026, 9, 24),
                new DateOnly(2026, 9, 24),
                new DateOnly(2026, 9, 26)
            };

            var result = FleetActivityChartBuilder.Build(today, 7, tripDates);

            Assert.Equal(2, result.First(d => d.Tanggal == new DateOnly(2026, 9, 24)).JumlahPerjalanan);
            Assert.Equal(1, result.First(d => d.Tanggal == new DateOnly(2026, 9, 26)).JumlahPerjalanan);
        }

        [Fact]
        public void Build_IgnoresTripsOutsideWindow()
        {
            var today = new DateOnly(2026, 9, 30);
            var tripDates = new[] { new DateOnly(2026, 9, 1) };

            var result = FleetActivityChartBuilder.Build(today, 7, tripDates);

            Assert.All(result, day => Assert.Equal(0, day.JumlahPerjalanan));
        }

        [Fact]
        public void BuildDaily_StartsAtGivenDate()
        {
            var start = new DateOnly(2026, 8, 27);

            var result = FleetActivityChartBuilder.BuildDaily(start, 30, Array.Empty<DateOnly>());

            Assert.Equal(30, result.Count);
            Assert.Equal(start, result.First().Tanggal);
            Assert.Equal(new DateOnly(2026, 9, 25), result.Last().Tanggal);
        }

        [Fact]
        public void BuildMonthly_AggregatesTripsPerMonth()
        {
            var start = new DateOnly(2025, 10, 1);
            var tripDates = new[]
            {
                new DateOnly(2025, 10, 3),
                new DateOnly(2025, 10, 20),
                new DateOnly(2026, 9, 25)
            };

            var result = FleetActivityChartBuilder.BuildMonthly(start, 12, tripDates);

            Assert.Equal(12, result.Count);
            Assert.Equal(new DateOnly(2025, 10, 1), result.First().Tanggal);
            Assert.Equal(2, result.First().JumlahPerjalanan);
            Assert.Equal(1, result.Last().JumlahPerjalanan);
            Assert.Equal(new DateOnly(2026, 9, 1), result.Last().Tanggal);
        }
    }
}
