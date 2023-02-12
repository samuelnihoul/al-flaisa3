using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

 public struct DepartureDate
{
public int day
        { get; set; }
    public int month
    { get; set; }
    public int year { get; set; }
}
public class Flight { 
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public string? OperatingCarrierCode { get; set; }
    public int FlightNumber { get; set; }
    public DepartureDate departureDate { get; set; }
}

public class FlightList
{
    public List<Flight>? Flights { get; set; }
}

public class EmissionResult
{
    // Your code to parse the JSON response from the API
}
[Route("api/[controller]")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public FlightController(IConfiguration configuration) { _configuration = configuration; }
    
    [HttpGet]
    public async Task<IActionResult> GetEmissions()
    {
        var apiKey = _configuration.GetValue<string>("API_KEY");
        var flights = new List<Flight>
        {
            new Flight
            {
                Origin = "ZRH",
                Destination = "CDG",
                OperatingCarrierCode = "AF",
                FlightNumber= 1115,
                departureDate= new DepartureDate {year = 2023, month = 11, day = 1}
            },
            new Flight
            {
                Origin = "CDG",
                Destination = "BOS",
                OperatingCarrierCode = "AF",
                FlightNumber = 334,
                departureDate = new DepartureDate {year = 2023, month = 11, day = 1}
            },
            new Flight
            {
                Origin = "ZRH",
                Destination = "BOS",
                OperatingCarrierCode = "LX",
                FlightNumber = 54,
                departureDate = new DepartureDate {year = 2023, month = 7, day = 1}
            }
        };

        var requestBody = JsonConvert.SerializeObject(new { flights });

        using (var httpClient = new HttpClient())
            
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var response = await httpClient.PostAsync(
                $"https://travelimpactmodel.googleapis.com/v1/flights:computeFlightEmissions?key={apiKey}",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.DeserializeObject(responseBody));
            }

            return BadRequest();
        }
    }
}

