namespace RouteOptimization.Api.Models;

public class RouteRequest
{
    public string[] Addresses { get; set; } = Array.Empty<string>();
    public string? StartingPoint { get; set; }
}

public class RouteResponse
{
    public string[] OptimizedAddresses { get; set; } = Array.Empty<string>();
    public double TotalDistance { get; set; }
    public int TotalDuration { get; set; }
    public string? Polyline { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AddressPair
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int Duration { get; set; }
    public string? Polyline { get; set; }
}
