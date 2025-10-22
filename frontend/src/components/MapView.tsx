import React, { useEffect, useRef, useState } from "react";
import { Loader } from "@googlemaps/js-api-loader";

interface MapViewProps {
  addresses: string[];
  isLoading: boolean;
}

const MapView: React.FC<MapViewProps> = ({ addresses, isLoading }) => {
  const mapRef = useRef<HTMLDivElement>(null);
  const mapInstanceRef = useRef<google.maps.Map | null>(null);
  const markersRef = useRef<google.maps.Marker[]>([]);
  const polylineRef = useRef<google.maps.Polyline | null>(null);
  const [mapLoaded, setMapLoaded] = useState(false);

  useEffect(() => {
    const initMap = async () => {
      const apiKey = process.env.REACT_APP_GOOGLE_MAPS_API_KEY;
      if (!apiKey || apiKey.trim() === "") {
        console.error("Google Maps API key is not configured");
        return;
      }
      const loader = new Loader({
        apiKey: process.env.REACT_APP_GOOGLE_MAPS_API_KEY || "",
        version: "weekly",
        libraries: ["places", "geometry"],
      });

      try {
        const google = await loader.load();

        if (mapRef.current && !mapInstanceRef.current) {
          mapInstanceRef.current = new google.maps.Map(mapRef.current, {
            center: { lat: 55.6713366, lng: 12.5114235 }, // Default to CPH (55.6713366,12.5114235)
            zoom: 10,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
          });

          // Wait for the map to be fully loaded using the idle event
          google.maps.event.addListenerOnce(
            mapInstanceRef.current,
            "idle",
            () => {
              setMapLoaded(true);
            }
          );
        }
      } catch (error) {
        console.error("Error loading Google Maps:", error);
      }
    };

    initMap();
  }, []);

  useEffect(() => {
    // Skip map operations if the map isn't ready or there are no addresses to display
    if (!mapLoaded) {
      return;
    }
    if (!mapInstanceRef.current) {
      return;
    }
    if (addresses.length === 0) {
      // DON'T clear markers/polyline when there are no addresses - keep the map as is
      return;
    }

    // Clear existing markers and polyline if we have addresses to process
    markersRef.current.forEach((marker) => marker.setMap(null));
    markersRef.current = [];

    // Clear existing route renderer
    if (polylineRef.current) {
      if (polylineRef.current.setMap) {
        polylineRef.current.setMap(null);
      }
      polylineRef.current = null;
    }

    // Use DirectionsService to render route from address sequence (works for both original and optimized)
    if (addresses.length > 0) {
      const directionsService = new google.maps.DirectionsService();
      const directionsRenderer = new google.maps.DirectionsRenderer();

      // Set up waypoints (all addresses except first and last)
      const waypoints =
        addresses.length > 2
          ? addresses.slice(1, -1).map((addr) => ({ location: addr }))
          : [];

      const request = {
        origin: addresses[0],
        destination: addresses[addresses.length - 1],
        waypoints: waypoints,
        travelMode: google.maps.TravelMode.DRIVING,
        optimizeWaypoints: false, // Don't re-optimize, use the order we provide
      };

      directionsService.route(request, (result, status) => {
        if (status === "OK" && mapInstanceRef.current) {
          directionsRenderer.setMap(mapInstanceRef.current);
          directionsRenderer.setDirections(result);

          // Store the renderer reference to clear it later
          polylineRef.current = directionsRenderer as any;
        }
      });
    }
  }, [addresses, mapLoaded]);

  /*if (isLoading) {
    return (
      <div className="card">
        <h3>Map</h3>
        <div className="loading">Loading map...</div>
      </div>
    );
  }*/

  return (
    <div className="card">
      <h3>Route Map</h3>

      <div
        ref={mapRef}
        className="map-container"
        style={{
          height: "400px",
          width: "500px", // Change from "100%" to explicit pixels
          border: "1px solid #ccc",
          backgroundColor: "#f0f0f0",
        }}
      />
      {addresses.length === 0 && (
        <p style={{ textAlign: "center", color: "#666", marginTop: "20px" }}>
          Add addresses to see them on the map
        </p>
      )}
    </div>
  );
};

export default MapView;
