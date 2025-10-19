using RouteOptimization.Api.Models;

namespace RouteOptimization.Api.Services;

public class RouteOptimizerService : IRouteOptimizerService
{
    private readonly IGoogleRoutesService _googleRoutesService;

    public RouteOptimizerService(IGoogleRoutesService googleRoutesService)
    {
        _googleRoutesService = googleRoutesService;
    }

    public async Task<RouteResponse> OptimizeRouteAsync(RouteRequest request)
    {
        try
        {
            if (request.Addresses == null || request.Addresses.Length < 2)
            {
                return new RouteResponse
                {
                    ErrorMessage = "At least 2 addresses are required for route optimization"
                };
            }

            // Clean and validate addresses
            var addresses = request.Addresses.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
            if (addresses.Length < 2)
            {
                return new RouteResponse
                {
                    ErrorMessage = "At least 2 valid addresses are required"
                };
            }

            // First address is the starting point, rest are delivery addresses
            var startingPoint = addresses[0];
            var deliveryAddresses = addresses.Skip(1).ToArray();

            // Implement Nearest Neighbor Algorithm with distance matrix
            return await OptimizeWithNearestNeighbor(deliveryAddresses, startingPoint);
        }
        catch (Exception ex)
        {
            return new RouteResponse
            {
                ErrorMessage = $"Error optimizing route: {ex.Message}"
            };
        }
    }

    private async Task<RouteResponse> OptimizeWithNearestNeighbor(string[] addresses, string startingPoint)
    {
        // Create array that includes starting point
        var allAddresses = new List<string> { startingPoint };
        allAddresses.AddRange(addresses);
        
        // Get distance matrix for all pairs of addresses (including starting point)
        var distanceMatrix = await _googleRoutesService.GetDistanceMatrixAsync(allAddresses.ToArray());
        
        // Create a lookup for distances
        var distanceLookup = new Dictionary<string, Dictionary<string, AddressPair>>();
        foreach (var pair in distanceMatrix)
        {
            if (!distanceLookup.ContainsKey(pair.Origin))
                distanceLookup[pair.Origin] = new Dictionary<string, AddressPair>();
            distanceLookup[pair.Origin][pair.Destination] = pair;
        }

        // Implement Nearest Neighbor Algorithm
        var unvisited = addresses.ToList(); // All delivery addresses (starting point not included)
        var optimizedRoute = new List<string> { startingPoint };
        var currentLocation = startingPoint;
        double totalDistance = 0;
        int totalDuration = 0;
        var polylines = new List<string>();

        while (unvisited.Count > 0)
        {
            // Find nearest unvisited address
            var nearestAddress = FindNearestAddress(currentLocation, unvisited, distanceLookup);
            var distanceInfo = distanceLookup[currentLocation][nearestAddress];
            
            optimizedRoute.Add(nearestAddress);
            totalDistance += distanceInfo.Distance;
            totalDuration += distanceInfo.Duration;
            
            if (!string.IsNullOrEmpty(distanceInfo.Polyline))
            {
                polylines.Add(distanceInfo.Polyline);
            }

            unvisited.Remove(nearestAddress);
            currentLocation = nearestAddress;
        }

        // Optionally return to starting point
        if (optimizedRoute.Count > 1 && distanceLookup.ContainsKey(currentLocation) && 
            distanceLookup[currentLocation].ContainsKey(startingPoint))
        {
            var returnInfo = distanceLookup[currentLocation][startingPoint];
            totalDistance += returnInfo.Distance;
            totalDuration += returnInfo.Duration;
            
            if (!string.IsNullOrEmpty(returnInfo.Polyline))
            {
                polylines.Add(returnInfo.Polyline);
            }
        }

        return new RouteResponse
        {
            OptimizedAddresses = optimizedRoute.ToArray(),
            TotalDistance = totalDistance,
            TotalDuration = totalDuration,
            Polyline = string.Join("", polylines) // Simplified - in reality you'd need to merge polylines properly
        };
    }

    private string FindNearestAddress(string currentLocation, List<string> unvisited, 
        Dictionary<string, Dictionary<string, AddressPair>> distanceLookup)
    {
        var minDistance = double.MaxValue;
        string nearestAddress = unvisited[0];

        foreach (var address in unvisited)
        {
            if (distanceLookup.ContainsKey(currentLocation) && 
                distanceLookup[currentLocation].ContainsKey(address))
            {
                var distance = distanceLookup[currentLocation][address].Distance;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestAddress = address;
                }
            }
        }

        return nearestAddress;
    }
}
