namespace FlightStorageService.Repository
{
    using FlightStorageService.Models;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using FlightStorageService.Helpers;

    public class FlightRepository
    {
        private readonly string _connectionString;

        public FlightRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
