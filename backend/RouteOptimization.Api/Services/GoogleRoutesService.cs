using RouteOptimization.Api.Models;
using Newtonsoft.Json;
using System.Text;

namespace RouteOptimization.Api.Services;

public class GoogleRoutesService : IGoogleRoutesService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;

    public GoogleRoutesService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["GoogleMaps:ApiKey"] ?? throw new InvalidOperationException("Google Maps API key not configured");
    }

    public async Task<AddressPair> GetDistanceAndDurationAsync(string origin, string destination)
    {
        var url = $"https://routes.googleapis.com/directions/v2:computeRoutes";
        
        var requestBody = new
        {
            origin = new { address = origin },
            destination = new { address = destination },
            travelMode = "DRIVE",
            routingPreference = "TRAFFIC_AWARE",
            computeAlternativeRoutes = false,
            routeModifiers = new
            {
                avoidTolls = false,
                avoidHighways = false,
                avoidFerries = false
            },
            languageCode = "en-US",
            units = "METRIC"
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        request.Headers.Add("X-Goog-Api-Key", _apiKey);
        request.Headers.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

        try
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var routeData = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (routeData?.routes?.Count > 0)
            {
                var route = routeData.routes[0];
                return new AddressPair
                {
                    Origin = origin,
                    Destination = destination,
                    Distance = (double)route.distanceMeters / 1000.0, // Convert to km
                    Duration = (int)route.duration.Replace("s", ""), // Remove 's' suffix and parse
                    Polyline = route.polyline?.encodedPolyline
                };
            }

            throw new Exception("No route found between the specified addresses");
        }
        catch (HttpRequestException ex)
        {
              // Check for specific API key errors
            if (ex.Message.Contains("403") || ex.Message.Contains("API_KEY_INVALID") || ex.Message.Contains("API_KEY_NOT_FOUND"))
            {
                throw new Exception("Invalid or missing Google Maps API key. Please check your configuration.");
            }
            throw new Exception($"Error calling Google Routes API: {ex.Message}");
        }
    }

    public async Task<List<AddressPair>> GetDistanceMatrixAsync(string[] addresses)
    {
        var pairs = new List<AddressPair>();
        
        // For simplicity, we'll get distances from each address to every other address
        // In a real implementation, you might want to optimize this with a distance matrix API
        for (int i = 0; i < addresses.Length; i++)
        {
            for (int j = 0; j < addresses.Length; j++)
            {
                if (i != j)
                {
                    var pair = await GetDistanceAndDurationAsync(addresses[i], addresses[j]);
                    pairs.Add(pair);
                }
            }
        }

        return pairs;
    }
}
