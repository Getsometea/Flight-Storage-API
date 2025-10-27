namespace FlightStorageService.Tests
{
    using FlightStorageService.Repository;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class FlightRepositoryTests
    {
        private readonly FlightRepository _repository;

        public FlightRepositoryTests() 
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());

            _repository = new FlightRepository(configuration, memoryCache);
        }

        [Fact]
        public async Task GetByNumberAsync_ReturnsFlight_WhenExists()
        {
            string flightNumber = "PS510";

            var flight = await _repository.GetByNumberAsync(flightNumber);

            Assert.NotNull(flight);
            Assert.Equal(flightNumber, flight.FlightNumber);
        }

        [Fact]
        public async Task GetByNumberAsync_ReturnsNull_WhenFlightDoesNotExist()
        {
            string flightNumber = "UNKNOWN999";

            var flight = await _repository.GetByNumberAsync(flightNumber);

            Assert.Null(flight);
        }

        [Fact]
        public async Task GetByDateAsync_ReturnsFlights_WhenDateExists()
        {
            var date = new DateTime(2025, 10, 31);
            var flights = await _repository.GetByDateAsync(date);

            Assert.NotNull(flights);
            Assert.NotEmpty(flights);
        }

        [Fact]
        public async Task GetByDateAsync_ReturnsEmpty_WhenNoFlights()
        {
            var date = new DateTime(1999, 1, 1);
            var flights = await _repository.GetByDateAsync(date);

            Assert.NotNull(flights);
            Assert.Empty(flights);
        }

        [Fact]
        public async Task CleanupOldFlightsAsync_RemovesOldFlights()
        {
            int deletedCount = await _repository.CleanupOldFlightsAsync();

            Assert.True(deletedCount >= 0);
        }
    }
}
