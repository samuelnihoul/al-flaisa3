using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public class FlightEmission
{
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string OperatingCarrierCode { get; set; }
    public int FlightNumber { get; set; }
    public DateTime DepartureDate { get; set; }
}

public class FlightEmissionRequest
{
    public List<FlightEmission> Flights { get; set; }
}

public class EmissionResult
{
    // Your code to parse the JSON response from the API
}

public class FlightEmissionsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    public FlightEmissionsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = configuration.GetValue<string>("API_KEY");
    }

    [HttpPost]
    public async Task<ActionResult<EmissionResult>> ComputeFlightEmissions([FromBody] FlightEmissionRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://travelimpactmodel.googleapis.com/v1/flights:computeFlightEmissions");
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        client.DefaultRequestHeaders.Add("key", _apiKey);

        var requestJson = JsonConvert.SerializeObject(request);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(client.BaseAddress, content);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest();
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<EmissionResult>(responseJson);

        return result;
    }
}

