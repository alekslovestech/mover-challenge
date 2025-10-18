import React from "react";
import { RouteResponse } from "../types";

interface ResultsPanelProps {
  result: RouteResponse | null;
  isLoading: boolean;
  error: string | null;
}

const ResultsPanel: React.FC<ResultsPanelProps> = ({
  result,
  isLoading,
  error,
}) => {
  if (isLoading) {
    return (
      <div className="card">
        <h3>Results</h3>
        <div className="loading">Optimizing route...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="card">
        <h3>Results</h3>
        <div className="error">{error}</div>
      </div>
    );
  }

  if (!result) {
    return (
      <div className="card">
        <h3>Results</h3>
        <p style={{ color: "#666", textAlign: "center" }}>
          Optimize a route to see results here
        </p>
      </div>
    );
  }

  if (result.errorMessage) {
    return (
      <div className="card">
        <h3>Results</h3>
        <div className="error">{result.errorMessage}</div>
      </div>
    );
  }

  const formatDuration = (seconds: number): string => {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);

    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    }
    return `${minutes}m`;
  };

  return (
    <div className="card">
      <h3>Optimized Route</h3>

      <div className="route-info">
        <p>
          <strong>Total Distance:</strong> {result.totalDistance.toFixed(2)} km
        </p>
        <p>
          <strong>Estimated Time:</strong>{" "}
          {formatDuration(result.totalDuration)}
        </p>
      </div>

      <div className="route-sequence">
        <h4>Delivery Sequence:</h4>
        <ol>
          {result.optimizedAddresses.map((address, index) => (
            <li key={index}>
              <strong>{index === 0 ? "Start" : `Stop ${index}`}:</strong>{" "}
              {address}
            </li>
          ))}
        </ol>
      </div>
    </div>
  );
};

export default ResultsPanel;
