# Route Optimization Application

A fullstack application that optimizes delivery routes using Google Routes API and the Nearest Neighbor algorithm. Built with C# ASP.NET Core Web API backend and React TypeScript frontend.

## Features

- **Route Optimization**: Implements the Nearest Neighbor algorithm to find optimal delivery routes
- **Google Maps Integration**: Visualizes routes on an interactive map with markers and polylines
- **Real-time Distance Calculation**: Uses Google Routes API for accurate distance and time estimates
- **User-friendly Interface**: Clean, responsive UI for adding addresses and viewing results
- **Error Handling**: Comprehensive error handling for API failures and invalid inputs

## Tech Stack

### Backend

- **C# ASP.NET Core 8.0** - Web API framework
- **Google Routes API** - Distance and route calculation
- **Newtonsoft.Json** - JSON serialization
- **Swagger/OpenAPI** - API documentation

### Frontend

- **React 18** with TypeScript
- **Google Maps JavaScript API** - Map visualization
- **Axios** - HTTP client
- **CSS3** - Styling and responsive design

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 16+](https://nodejs.org/)
- [Google Cloud Platform account](https://cloud.google.com/) with Routes API enabled
- Google Maps API key with the following APIs enabled:
  - Routes API
  - Maps JavaScript API
  - Geocoding API

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd route-optimization-app
```

### 2. Backend Setup

1. Navigate to the backend directory:

```bash
cd backend/RouteOptimization.Api
```

2. Install dependencies:

```bash
dotnet restore
```

3. Configure the Google Maps API key:

   - Open `appsettings.json` and `appsettings.Development.json`
   - Replace `YOUR_GOOGLE_MAPS_API_KEY_HERE` with your actual Google Maps API key

4. Run the backend:

```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or the port shown in the console).

### 3. Frontend Setup

1. Navigate to the frontend directory:

```bash
cd frontend
```

2. Install dependencies:

```bash
npm install
```

3. Configure environment variables:

   - Copy `env.example` to `.env`
   - Replace `your_google_maps_api_key_here` with your Google Maps API key
   - Update `REACT_APP_API_URL` if your backend runs on a different port

4. Start the development server:

```bash
npm start
```

The frontend will be available at `http://localhost:3000`.

## API Endpoints

### POST /api/route/optimize

Optimizes a route for the given addresses.

**Request Body:**

```json
{
  "addresses": [
    "123 Main St, New York, NY",
    "456 Oak Ave, Brooklyn, NY",
    "789 Pine St, Queens, NY"
  ],
  "startingPoint": "100 Central Park, New York, NY"
}
```

**Response:**

```json
{
  "optimizedAddresses": [
    "100 Central Park, New York, NY",
    "123 Main St, New York, NY",
    "456 Oak Ave, Brooklyn, NY",
    "789 Pine St, Queens, NY"
  ],
  "totalDistance": 15.7,
  "totalDuration": 1800,
  "polyline": "encoded_polyline_string"
}
```

### GET /api/route/health

Health check endpoint.

**Response:**

```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## How to Demo

1. **Start both services** (backend and frontend) following the setup instructions above.

2. **Open the application** in your browser at `http://localhost:3000`.

3. **Add delivery addresses**:

   - Enter at least 2 delivery addresses in the form
   - Optionally specify a starting point (defaults to first address)
   - Click "Add Address" to add more destinations

4. **Optimize the route**:

   - Click "Optimize Route" button
   - Wait for the algorithm to calculate the optimal sequence
   - View results showing total distance, time, and delivery sequence

5. **View the map**:
   - See markers for each delivery point (numbered in sequence)
   - Red marker indicates the starting point
   - Blue markers show delivery destinations
   - Route polyline connects all points in the optimized order

## Example Addresses for Testing

Here are some sample addresses you can use for testing:

```
Starting Point: Times Square, New York, NY
Addresses:
- Central Park, New York, NY
- Brooklyn Bridge, New York, NY
- Statue of Liberty, New York, NY
- Empire State Building, New York, NY
- High Line, New York, NY
```

## Algorithm Details

The application uses the **Nearest Neighbor Algorithm** for route optimization:

1. **Start** from the specified starting point (or first address)
2. **Find** the nearest unvisited address
3. **Move** to that address and mark it as visited
4. **Repeat** until all addresses are visited
5. **Return** to the starting point (optional)

**Time Complexity**: O(n²) where n is the number of addresses
**Space Complexity**: O(n)

This heuristic approach provides good results for small to medium-sized delivery routes and is computationally efficient.

## Error Handling

The application handles various error scenarios:

- **Invalid addresses**: Unrecognizable or malformed addresses
- **API failures**: Google Routes API errors, network issues
- **Empty inputs**: Validation for minimum address requirements
- **Rate limiting**: Google API quota exceeded
- **Network errors**: Connection timeouts, server unavailable

## Future Improvements

- **Advanced algorithms**: 2-opt, genetic algorithms for better optimization
- **Database integration**: Store routes and delivery history
- **User authentication**: Multi-user support with saved routes
- **Real-time updates**: Live traffic data integration
- **Mobile app**: React Native version for delivery drivers
- **Batch processing**: Handle multiple route optimizations
- **Caching**: Cache distance calculations for repeated addresses

## Development Notes

### Backend Architecture

- **Clean Architecture**: Separation of concerns with services and controllers
- **Dependency Injection**: Proper IoC container usage
- **Async/Await**: Non-blocking API calls to Google services
- **Error Handling**: Comprehensive exception handling and logging

### Frontend Architecture

- **Component-based**: Reusable React components
- **TypeScript**: Type safety and better development experience
- **Responsive Design**: Mobile-friendly interface
- **State Management**: Local state with hooks (could be enhanced with Redux for larger apps)

## Troubleshooting

### Common Issues

1. **CORS errors**: Ensure the backend CORS policy allows your frontend URL
2. **Google Maps not loading**: Check your API key and enabled APIs
3. **Route optimization fails**: Verify addresses are valid and Google Routes API is enabled
4. **Port conflicts**: Change ports in configuration if default ports are in use

### Debug Mode

Enable detailed logging by setting the log level to `Debug` in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

## License

This project is created for interview purposes and demonstrates fullstack development skills with modern technologies.
