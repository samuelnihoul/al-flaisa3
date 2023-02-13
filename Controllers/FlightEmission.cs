using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

public class DepartureDate
{
    public int year { get; set; }
    public int month
    { get; set; }
    public int day { get; set; }
}
public class Flight
{
    public string? origin { get; set; }
    public string? destination { get; set; }
    public string? operating_carrier_code { get; set; }
    public int flight_number { get; set; }
    public DepartureDate? departure_date { get; set; }
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
                origin = "ZRH",
                destination = "CDG",
                operating_carrier_code = "AF",
                flight_number= 1115,
                departure_date=  new DepartureDate{year = 2023, month = 11,day = 1}
            },new Flight
            {origin="CDG",
            destination="BOS",operating_carrier_code="AF",flight_number=334,departure_date=new DepartureDate{year=2023,month=11,day=1}}
                 ,new Flight
                 {origin="ZRH" ,destination="BOS",operating_carrier_code="LX",


                flight_number=54,departure_date=new DepartureDate
                {
                year=2023,
            day=1,
            month=11
                }
                 } };


        var requestBody = JsonConvert.SerializeObject(new { flights });

        using (var httpClient = new HttpClient())

        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

            var response = await httpClient.PostAsync(
                $"https://travelimpactmodel.googleapis.com/v1/flights:computeFlightEmissions?key={apiKey}",
                new StringContent(requestBody, Encoding.UTF8, "application/json"));

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Ok(JsonConvert.DeserializeObject(responseBody));
            }

            return BadRequest(JsonConvert.DeserializeObject(responseBody));
        }
    }
}

