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

            // Implement Nearest Neighbor Algorithm with distance matrix
            return await OptimizeWithNearestNeighbor(addresses);
        }
        catch (Exception ex)
        {
            return new RouteResponse
            {
                ErrorMessage = $"Error optimizing route: {ex.Message}"
            };
        }
    }

    private async Task<RouteResponse> OptimizeWithNearestNeighbor(string[] addresses)
    {
        // First address is the starting point, rest are delivery addresses
        var startingPoint = addresses[0];
        var deliveryAddresses = addresses.Skip(1).ToArray();
        
        // Get distance matrix for all pairs of addresses (including starting point)
        var distanceMatrix = await _googleRoutesService.GetDistanceMatrixAsync(addresses);
        
        // Create 2D distance matrix
        var distanceMatrix2D = BuildDistanceMatrix(distanceMatrix, addresses);

        // Convert to indices for optimization
        var addressToIndex = addresses.Select((addr, i) => (addr, i)).ToDictionary(x => x.addr, x => x.i);
        var startingIndex = addressToIndex[startingPoint];
        var deliveryIndices = deliveryAddresses.Select(addr => addressToIndex[addr]).ToList();
        
        // Implement Nearest Neighbor Algorithm with indices
        var unvisitedIndices = deliveryIndices.ToList();
        var optimizedIndices = new List<int> { startingIndex };
        var currentIndex = startingIndex;
        double totalDistance = 0;
        int totalDuration = 0;
        var polylines = new List<string>();

        while (unvisitedIndices.Count > 0)
        {
            // Find nearest unvisited index
            var nearestIndex = FindNearestIndex(currentIndex, unvisitedIndices, distanceMatrix2D);
            
            // Get distance info from original matrix
            var currentAddress = addresses[currentIndex];
            var nearestAddress = addresses[nearestIndex];
            var distanceInfo = distanceMatrix.First(p => p.Origin == currentAddress && p.Destination == nearestAddress);
            
            optimizedIndices.Add(nearestIndex);
            totalDistance += distanceInfo.Distance;
            totalDuration += distanceInfo.Duration;
            
            if (!string.IsNullOrEmpty(distanceInfo.Polyline))
            {
                polylines.Add(distanceInfo.Polyline);
            }

            unvisitedIndices.Remove(nearestIndex);
            currentIndex = nearestIndex;
        }

        // Optionally return to starting point
        if (optimizedIndices.Count > 1)
        {
            var currentAddress = addresses[currentIndex];
            var returnInfo = distanceMatrix.FirstOrDefault(p => p.Origin == currentAddress && p.Destination == startingPoint);
            if (returnInfo != null)
            {
                totalDistance += returnInfo.Distance;
                totalDuration += returnInfo.Duration;
                
                if (!string.IsNullOrEmpty(returnInfo.Polyline))
                {
                    polylines.Add(returnInfo.Polyline);
                }
            }
        }

        // Convert optimized indices back to addresses
        var optimizedAddresses = optimizedIndices.Select(i => addresses[i]).ToArray();

        return new RouteResponse
        {
            OptimizedAddresses = optimizedAddresses,
            TotalDistance = totalDistance,
            TotalDuration = totalDuration,
            Polyline = string.Join("", polylines) // Simplified - in reality you'd need to merge polylines properly
        };
    }

    private double[,] BuildDistanceMatrix(List<AddressPair> distancePairs, string[] addresses)
    {
        var addressToIndex = addresses.Select((addr, i) => (addr, i)).ToDictionary(x => x.addr, x => x.i);
        var matrix = new double[addresses.Length, addresses.Length];
        
        foreach (var pair in distancePairs)
        {
            var fromIndex = addressToIndex[pair.Origin];
            var toIndex = addressToIndex[pair.Destination];
            matrix[fromIndex, toIndex] = pair.Distance;
        }
        
        return matrix;
    }

    private int FindNearestIndex(int currentIndex, List<int> unvisitedIndices, double[,] distanceMatrix)
    {
        var minDistance = double.MaxValue;
        int nearestIndex = unvisitedIndices[0];

        foreach (var addressIndex in unvisitedIndices)
        {
            var distance = distanceMatrix[currentIndex, addressIndex];
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = addressIndex;
            }
        }

        return nearestIndex;
    }
}
