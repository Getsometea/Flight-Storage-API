namespace FlightClientApp.Controllers
{
    using FlightStorageService.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http;
    using System.Text.Json;

    public class FlightsController : Controller
    {
        private readonly HttpClient _client;

        public FlightsController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("flightsApi");
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchByNumber(string flightNumber)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
            {
                ViewBag.Error = "Enter the flight number.";
                return View("Search");
            }

            var response = await _client.GetAsync($"api/flights/{flightNumber}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Flight not found.";
                return View("Search");
            }

            var json = await response.Content.ReadAsStringAsync();
            var flight = JsonSerializer.Deserialize<Flight>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("Search", flight);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByDeparture(string city, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Enter your departure city and date.";
                return View("SearchByDeparture");
            }

            var response = await _client.GetAsync($"api/flights/departure?city={city}&date={date:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "No flights found.";
                return View("SearchByDeparture");
            }

            var json = await response.Content.ReadAsStringAsync();
            var flights = JsonSerializer.Deserialize<List<Flight>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("SearchByDeparture", flights);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByArrival(string city, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Enter your arrival city and date.";
                return View("SearchByArrival");
            }

            var response = await _client.GetAsync($"api/flights/arrival?city={city}&date={date:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "No flights found.";
                return View("SearchByArrival");
            }

            var json = await response.Content.ReadAsStringAsync();
            var flights = JsonSerializer.Deserialize<List<Flight>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("SearchByArrival", flights);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByDate(DateTime date)
        {  
            if (date == default)
            {
                ViewBag.Error = "Select a date.";
                return View("SearchByDate");
            }

            var response = await _client.GetAsync($"api/flights?date={date:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "No flights found for the selected date.";
                return View("SearchByDate");
            }

            var json = await response.Content.ReadAsStringAsync();
            var flights = JsonSerializer.Deserialize<List<Flight>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View("SearchByDate", flights);


        }

    }
}
