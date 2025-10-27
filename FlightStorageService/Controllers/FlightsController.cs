namespace FlightStorageService.Controllers
{
    using FlightStorageService.Helpers;
    using FlightStorageService.Repository;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly FlightRepository _repository;
        private readonly ILogger<FlightsController> _logger;

        public FlightsController(FlightRepository repository, ILogger<FlightsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{flightNumber}")]
        public async Task<IActionResult> GetByNumber(string flightNumber)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
            {
                _logger.LogWarning("Attempt to request a flight without a number");
                return BadRequest("The flight number is required.");
            }

            try
            {
                var flight = await _repository.GetByNumberAsync(flightNumber);

                if (flight == null)
                {
                    _logger.LogInformation($"Flight {flightNumber} not found");
                    return NotFound($"Flight {flightNumber} not found.");
                }

                _logger.LogInformation($"Flight {flightNumber} found successfully");
                return Ok(flight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request for flight {flightNumber}");
                return StatusCode(500, "Error accessing the database.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            if (date == default)
            {
                _logger.LogWarning("Attempt to request without specifying a date.");
                return BadRequest("The date cannot be empty.");
            }

            if (!DateValidator.IsDateValid(date))
            {
                _logger.LogWarning("Invalid date specified: {Date}", date);
                return BadRequest("The date must be between 1753 and 9999.");
            }

            try
            {
                _logger.LogInformation("Flight request for {Date}.", date);

                var flights = await _repository.GetByDateAsync(date);
                if (flights == null || !flights.Any())
                {
                    _logger.LogInformation("No flights found for {Date}.", date);
                    return NotFound("No flights found.");
                }

                _logger.LogInformation("Found {Count} flights on {Date}.", flights.Count(), date);
                return Ok(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flights on {Date}.", date);
                return StatusCode(500, "Error processing request.");
            }
        }

        [HttpGet("departure")]
        public async Task<IActionResult> GetByDepartureCity([FromQuery] string city, [FromQuery] DateTime date)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                _logger.LogWarning("Request without specifying the city of departure.");
                return BadRequest("The city of departure is mandatory.");
            }

            if (!DateValidator.IsDateValid(date))
            {
                _logger.LogWarning("Invalid date ({Date}) when searching by departure city.", date);
                return BadRequest("The date must be between 1753 and 9999.");
            }

            try
            {
                _logger.LogInformation("Request for flights departing from {City} on {Date}.", city, date);

                var flights = await _repository.GetByDepartureCityAndDateAsync(city, date);
                if (flights == null || !flights.Any())
                {
                    _logger.LogInformation("No flights found from {City} on {Date}.", city, date);
                    return NotFound("No flights found.");
                }

                _logger.LogInformation("Found {Count} flights departing from {City} on {Date}.", flights.Count(), city, date);
                return Ok(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for flights departing from {City} on {Date}.", city, date);
                return StatusCode(500, "Error processing request.");
            }
        }

        [HttpGet("arrival")]
        public async Task<IActionResult> GetByArrivalCity([FromQuery] string city, [FromQuery] DateTime date)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                _logger.LogWarning("Request without specifying the city of arrival.");
                return BadRequest("The city of arrival is mandatory.");
            }

            if (!DateValidator.IsDateValid(date))
            {
                _logger.LogWarning("Invalid date ({Date}) when searching by city of arrival.", date);
                return BadRequest("The date must be between 1753 and 9999.");
            }

            try
            {
                _logger.LogInformation("Request for flights arriving in {City} on {Date}.", city, date);

                var flights = await _repository.GetByArrivalCityAndDateAsync(city, date);
                if (flights == null || !flights.Any())
                {
                    _logger.LogInformation("No flights arriving in {City} on {Date} were found.", city, date);
                    return NotFound("No flights found.");
                }

                _logger.LogInformation("Found {Count} flights arriving in {City} on {Date}.", flights.Count(), city, date);
                return Ok(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for flights arriving in {City} on {Date}.", city, date);
                return StatusCode(500, "Error processing request.");
            }
        }
    }
}
