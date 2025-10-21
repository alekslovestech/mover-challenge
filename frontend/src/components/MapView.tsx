import React, { useEffect, useRef, useState } from "react";
import { Loader } from "@googlemaps/js-api-loader";

interface MapViewProps {
  addresses: string[];
  polyline?: string;
  isLoading: boolean;
}

const MapView: React.FC<MapViewProps> = ({
  addresses,
  polyline,
  isLoading,
}) => {
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
          setMapLoaded(true);
        }
      } catch (error) {
        console.error("Error loading Google Maps:", error);
      }
    };

    initMap();
  }, []);

  useEffect(() => {
    if (!mapLoaded || !mapInstanceRef.current || addresses.length === 0) {
      return;
    }

    // Clear existing markers and polyline
    markersRef.current.forEach((marker) => marker.setMap(null));
    markersRef.current = [];

    if (polylineRef.current) {
      polylineRef.current.setMap(null);
    }

    // Geocode addresses and add markers
    const geocoder = new google.maps.Geocoder();
    const bounds = new google.maps.LatLngBounds();
    let geocodedCount = 0;

    addresses.forEach((address, index) => {
      geocoder.geocode({ address }, (results, status) => {
        if (
          status === "OK" &&
          results &&
          results[0] &&
          mapInstanceRef.current
        ) {
          const location = results[0].geometry.location;

          const marker = new google.maps.Marker({
            position: location,
            map: mapInstanceRef.current,
            title: address,
            label: {
              text: (index + 1).toString(),
              color: "white",
              fontWeight: "bold",
            },
            icon: {
              url: `https://maps.google.com/mapfiles/ms/icons/${
                index === 0 ? "red" : "blue"
              }-dot.png`,
              scaledSize: new google.maps.Size(32, 32),
            },
          });

          markersRef.current.push(marker);
          bounds.extend(location);
          geocodedCount++;

          // Fit bounds when all addresses are geocoded
          if (geocodedCount === addresses.length) {
            mapInstanceRef.current.fitBounds(bounds);
          }
        }
      });
    });

    // Draw polyline if provided
    if (polyline && mapInstanceRef.current) {
      try {
        const decodedPath = google.maps.geometry.encoding.decodePath(polyline);
        polylineRef.current = new google.maps.Polyline({
          path: decodedPath,
          geodesic: true,
          strokeColor: "#FF0000",
          strokeOpacity: 1.0,
          strokeWeight: 3,
          map: mapInstanceRef.current,
        });
      } catch (error) {
        console.error("Error decoding polyline:", error);
      }
    }
  }, [addresses, polyline, mapLoaded]);

  if (isLoading) {
    return (
      <div className="card">
        <h3>Map</h3>
        <div className="loading">Loading map...</div>
      </div>
    );
  }

  return (
    <div className="card">
      <h3>Route Map</h3>
      <div ref={mapRef} className="map-container" />
      {addresses.length === 0 && (
        <p style={{ textAlign: "center", color: "#666", marginTop: "20px" }}>
          Add addresses to see them on the map
        </p>
      )}
    </div>
  );
};

export default MapView;
