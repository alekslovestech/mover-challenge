using RouteOptimization.Api.Models;

namespace RouteOptimization.Api.Services;

public interface IRouteOptimizerService
{
    Task<RouteResponse> OptimizeRouteAsync(RouteRequest request);
}
