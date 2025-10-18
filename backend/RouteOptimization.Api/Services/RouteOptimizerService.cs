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

            // Use starting point if provided, otherwise use first address
            var startingPoint = !string.IsNullOrWhiteSpace(request.StartingPoint) 
                ? request.StartingPoint 
                : addresses[0];

            // Implement Nearest Neighbor Algorithm
            var optimizedRoute = await OptimizeWithNearestNeighbor(addresses, startingPoint);

            return new RouteResponse
            {
                OptimizedAddresses = optimizedRoute.Addresses,
                TotalDistance = optimizedRoute.TotalDistance,
                TotalDuration = optimizedRoute.TotalDuration,
                Polyline = optimizedRoute.Polyline
            };
        }
        catch (Exception ex)
        {
            return new RouteResponse
            {
                ErrorMessage = $"Error optimizing route: {ex.Message}"
            };
        }
    }

    private async Task<OptimizedRoute> OptimizeWithNearestNeighbor(string[] addresses, string startingPoint)
    {
        var unvisited = addresses.Where(a => a != startingPoint).ToList();
        var route = new List<string> { startingPoint };
        var currentLocation = startingPoint;
        double totalDistance = 0;
        int totalDuration = 0;
        var polylines = new List<string>();

        while (unvisited.Count > 0)
        {
            var nearestAddress = await FindNearestAddress(currentLocation, unvisited);
            var distanceInfo = await _googleRoutesService.GetDistanceAndDurationAsync(currentLocation, nearestAddress);
            
            route.Add(nearestAddress);
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
        if (route.Count > 1)
        {
            var returnInfo = await _googleRoutesService.GetDistanceAndDurationAsync(currentLocation, startingPoint);
            totalDistance += returnInfo.Distance;
            totalDuration += returnInfo.Duration;
            
            if (!string.IsNullOrEmpty(returnInfo.Polyline))
            {
                polylines.Add(returnInfo.Polyline);
            }
        }

        return new OptimizedRoute
        {
            Addresses = route.ToArray(),
            TotalDistance = totalDistance,
            TotalDuration = totalDuration,
            Polyline = string.Join("", polylines) // Simplified - in reality you'd need to merge polylines properly
        };
    }

    private async Task<string> FindNearestAddress(string currentLocation, List<string> unvisited)
    {
        var minDistance = double.MaxValue;
        string nearestAddress = unvisited[0];

        foreach (var address in unvisited)
        {
            var distanceInfo = await _googleRoutesService.GetDistanceAndDurationAsync(currentLocation, address);
            if (distanceInfo.Distance < minDistance)
            {
                minDistance = distanceInfo.Distance;
                nearestAddress = address;
            }
        }

        return nearestAddress;
    }

    private class OptimizedRoute
    {
        public string[] Addresses { get; set; } = Array.Empty<string>();
        public double TotalDistance { get; set; }
        public int TotalDuration { get; set; }
        public string? Polyline { get; set; }
    }
}
