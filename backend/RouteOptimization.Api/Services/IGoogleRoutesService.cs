using RouteOptimization.Api.Models;

namespace RouteOptimization.Api.Services;

public interface IGoogleRoutesService {
    Task<AddressPair> GetDistanceAndDurationAsync(string origin, string destination);
    Task<List<AddressPair>> GetDistanceMatrixAsync(string[] addresses);
}
