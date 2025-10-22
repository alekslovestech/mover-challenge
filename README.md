# Route Optimization Application

A fullstack application that optimizes delivery routes using Google Routes API and the Nearest Neighbor algorithm.

## Features

- Route optimization using Nearest Neighbor algorithm
- Interactive map visualization with Google Maps
- Real-time distance and time calculations
- Clean, responsive UI

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 16+](https://nodejs.org/)
- Google Maps API key with Routes API, Maps JavaScript API, and Geocoding API enabled
- [Yarn](https://yarnpkg.com/) (or use npm as alternative)

## Quick Start

1. **Clone and install dependencies:**

   ```
   git clone https://github.com/alekslovestech/mover-challenge
   cd mover-challenge
   yarn setup
   ```

2. **Configure API keys:**

   - Backend: Update `backend/RouteOptimization.Api/appsettings.json`
   - Frontend: Create `frontend/.env` with `REACT_APP_GOOGLE_MAPS_API_KEY=your_key`

3. **Start the application:**

   `yarn start`

4. **Access the app:**
   - Frontend: http://localhost:3000
   - API Docs: http://localhost:5000/swagger

## How to Demo

1. Add delivery addresses (minimum 2)
2. Click "Optimize Route"
3. View the optimized route on the map with total distance/time
4. See the delivery sequence in the results panel

## Example Test Addresses

```
Starting Point: Times Square, New York, NY
Addresses:
- Central Park, New York, NY
- Brooklyn Bridge, New York, NY
- Statue of Liberty, New York, NY
- Empire State Building, New York, NY
```

## API Endpoints

- `POST /api/route/optimize` - Optimize route for given addresses
- `GET /api/route/health` - Health check

## Algorithm

Uses Nearest Neighbor algorithm (O(n²) complexity) to find optimal delivery sequence starting from the first address.
