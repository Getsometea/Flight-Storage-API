namespace FlightStorageService.Repository
{
    using FlightStorageService.Helpers;
    using FlightStorageService.Models;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Caching.Memory;
    using System.Data;

    public class FlightRepository
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        public FlightRepository(IConfiguration configuration, IMemoryCache cache)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _cache = cache;
        }

        public async Task<List<Flight>> GetAllFlightsAsync()
        {
            if (_cache.TryGetValue("allFlights", out List<Flight>? cachedFlights))
            {
                Console.WriteLine("Retrieved from cache");
                return cachedFlights!;
            }

            Console.WriteLine("Download from database...");

            List<Flight> flights = new();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Flights", connection);
                await connection.OpenAsync();
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    flights.Add(new Flight
                    {
                        FlightNumber = (string)reader["FlightNumber"],
                        DepartureAirportCity = (string)reader["DepartureAirportCity"],
                        ArrivalAirportCity = (string)reader["ArrivalAirportCity"],
                        DepartureDateTime = (DateTime)reader["DepartureDateTime"],
                        DurationMinutes = (int)reader["DurationMinutes"]
                    });
                }
            }


            _cache.Set("allFlights", flights, TimeSpan.FromMinutes(5));

            return flights;
        }
        public async Task<Flight?> GetByNumberAsync(string flightNumber)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetFlightByNumber", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@FlightNumber", flightNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapFlight(reader);

            return null;
        }

        public async Task<IEnumerable<Flight>> GetByDateAsync(DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetFlightBySpecificDate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Date", date);

            var flights = new List<Flight>();
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(new Flight
                {
                    FlightNumber = reader["FlightNumber"].ToString(),
                    DepartureAirportCity = reader["DepartureAirportCity"].ToString(),
                    ArrivalAirportCity = reader["ArrivalAirportCity"].ToString(),
                    DepartureDateTime = Convert.ToDateTime(reader["DepartureDateTime"]),
                    DurationMinutes = Convert.ToInt32(reader["DurationMinutes"])
                });
            }

            return flights;
        }


        public async Task<List<Flight>> GetByDepartureCityAndDateAsync(string city, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetFlightsByDepartureCityAndDate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@City", city);
            command.Parameters.AddWithValue("@Date", date);
            var flights = new List<Flight>();
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(new Flight
                {
                    FlightNumber = reader["FlightNumber"].ToString(),
                    DepartureAirportCity = reader["DepartureAirportCity"].ToString(),
                    ArrivalAirportCity = reader["ArrivalAirportCity"].ToString(),
                    DepartureDateTime = Convert.ToDateTime(reader["DepartureDateTime"]),
                    DurationMinutes = Convert.ToInt32(reader["DurationMinutes"])
                });
            }


            return flights;
        }

        public async Task<List<Flight>> GetByArrivalCityAndDateAsync(string city, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetFlightsByArrivalCityAndDate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@City", city);
            command.Parameters.AddWithValue("@Date", date);

            var flights = new List<Flight>();
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(new Flight
                {
                    FlightNumber = reader["FlightNumber"].ToString(),
                    DepartureAirportCity = reader["DepartureAirportCity"].ToString(),
                    ArrivalAirportCity = reader["ArrivalAirportCity"].ToString(),
                    DepartureDateTime = Convert.ToDateTime(reader["DepartureDateTime"]),
                    DurationMinutes = Convert.ToInt32(reader["DurationMinutes"])
                });
            }


            return flights;
        }

        private Flight MapFlight(SqlDataReader reader)
        {
            return new Flight
            {
                FlightNumber = reader["FlightNumber"].ToString() ?? "",
                DepartureDateTime = (DateTime)reader["DepartureDateTime"],
                DepartureAirportCity = reader["DepartureAirportCity"].ToString() ?? "",
                ArrivalAirportCity = reader["ArrivalAirportCity"].ToString() ?? "",
                DurationMinutes = (int)reader["DurationMinutes"]
            };
        }

        public async Task<int> CleanupOldFlightsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("CleanupOldFlights", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0);
        }
    }
}
