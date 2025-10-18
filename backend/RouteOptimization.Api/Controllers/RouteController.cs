using Microsoft.AspNetCore.Mvc;
using RouteOptimization.Api.Models;
using RouteOptimization.Api.Services;

namespace RouteOptimization.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteOptimizerService _routeOptimizerService;
    private readonly ILogger<RouteController> _logger;

    public RouteController(IRouteOptimizerService routeOptimizerService, ILogger<RouteController> logger)
    {
        _routeOptimizerService = routeOptimizerService;
        _logger = logger;
    }

    [HttpPost("optimize")]
    public async Task<ActionResult<RouteResponse>> OptimizeRoute([FromBody] RouteRequest request)
    {
        try
        {
            _logger.LogInformation("Received route optimization request for {AddressCount} addresses", 
                request.Addresses?.Length ?? 0);

            var result = await _routeOptimizerService.OptimizeRouteAsync(request);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                _logger.LogWarning("Route optimization failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }

            _logger.LogInformation("Route optimization completed successfully. Total distance: {Distance}km, Duration: {Duration}s", 
                result.TotalDistance, result.TotalDuration);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during route optimization");
            return StatusCode(500, new RouteResponse
            {
                ErrorMessage = "An unexpected error occurred while optimizing the route"
            });
        }
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
