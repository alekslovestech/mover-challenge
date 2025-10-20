using RouteOptimization.Api.Models;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;
using Newtonsoft.Json.Linq;

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
        var cfgKey = _configuration["GoogleMaps:ApiKey"];
        if (string.IsNullOrWhiteSpace(cfgKey) || cfgKey.Contains("${", StringComparison.Ordinal))
        {
            cfgKey = Environment.GetEnvironmentVariable("GOOGLE_MAPS_API_KEY");
        }
         _apiKey = cfgKey ?? throw new InvalidOperationException("Google Maps API key not configured");
    }

	public async Task<AddressPair> GetDistanceAndDurationAsync(string origin, string destination)
    {
		var url = $"https://routes.googleapis.com/directions/v2:computeRoutes?key={_apiKey}";

		// Resolve inputs to waypoints (placeId or latLng preferred)
		var resolvedOrigin = ResolveWaypoint(origin);
		var resolvedDestination = ResolveWaypoint(destination);

		var requestBody = new
		{
			origin = resolvedOrigin,
			destination = resolvedDestination,
			travelMode = "DRIVE",
			routingPreference = "TRAFFIC_AWARE"
		};

		var json = JsonConvert.SerializeObject(requestBody);
		var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };

        //request.Headers.Add("X-Goog-Api-Key", _apiKey);
		request.Headers.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

        try
        {
			var response = await _httpClient.SendAsync(request);
			var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google Routes API error {(int)response.StatusCode}: {responseContent}");
            }

			var routeData = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (routeData?.routes?.Count > 0)
            {
                var route = routeData.routes[0];
				// Parse duration which may be fractional like "123.4s"
				string durationStr = (string)route.duration;
				if (durationStr.EndsWith("s", StringComparison.Ordinal))
				{
					durationStr = durationStr.Substring(0, durationStr.Length - 1);
				}
				double secondsDouble = double.TryParse(durationStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var tmpSeconds) ? tmpSeconds : 0;
				int seconds = (int)Math.Round(secondsDouble);

				double distanceMeters = (double)route.distanceMeters;

				return new AddressPair
				{
					Origin = origin,
					Destination = destination,
					Distance = distanceMeters / 1000.0,
					Duration = seconds,
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

	private object ResolveWaypoint(string input)
	{
		// place_id:XYZ
		if (input.StartsWith("place_id:", StringComparison.OrdinalIgnoreCase))
		{
			return new { place_id = input.Substring("place_id:".Length) };
		}

		// lat,lng
		var parts = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 2
			&& double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat)
			&& double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lng))
		{
			return new { lat_lng = new { latitude = lat, longitude = lng } };
		}

		// Fallback: use address directly (let Routes API handle geocoding)
		return new { address = input };

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
