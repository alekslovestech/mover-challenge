import React, { useState } from "react";
import AddressForm from "./components/AddressForm";
import MapView from "./components/MapView";
import ResultsPanel from "./components/ResultsPanel";
import { routeApi } from "./services/api";
import { AddressItem, RouteResponse } from "./types";
import "./App.css";

function App() {
  const [addresses, setAddresses] = useState<AddressItem[]>([
    { id: "1", value: "Dragør Stationsplads, 2791 Dragør, Denmark" },
    { id: "2", value: "Lufthavnen, 2770 Kastrup, Denmark" },
  ]);
  const [startingPoint, setStartingPoint] = useState(
    "Finlandsgade 23, 2300 København, Denmark"
  );
  const [result, setResult] = useState<RouteResponse | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleOptimize = async () => {
    setIsLoading(true);
    setError(null);
    setResult(null);

    try {
      const validAddresses = addresses
        .map((addr) => addr.value.trim())
        .filter((addr) => addr !== "");

      if (validAddresses.length < 1) {
        throw new Error("At least 1 delivery address is required");
      }

      // Combine starting point and delivery addresses into single array
      const allAddresses = [startingPoint.trim(), ...validAddresses];

      const request = {
        addresses: allAddresses,
      };

      const response = await routeApi.optimizeRoute(request);
      setResult(response);
    } catch (err) {
      const errorMessage =
        err instanceof Error ? err.message : "An unexpected error occurred";
      setError(errorMessage);
      console.error("Route optimization error:", err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleReset = () => {
    setAddresses([
      { id: "1", value: "" },
      { id: "2", value: "" },
    ]);
    setStartingPoint("");
    setResult(null);
    setError(null);
  };

  return (
    <div className="App">
      <div className="container">
        <header style={{ textAlign: "center", marginBottom: "30px" }}>
          <h1>Route Optimization App</h1>
          <p>Optimize your delivery routes</p>
        </header>

        <div
          style={{
            display: "grid",
            gridTemplateColumns: "1fr 1fr",
            gap: "20px",
          }}
        >
          <div>
            <AddressForm
              addresses={addresses}
              onAddressesChange={setAddresses}
              onOptimize={handleOptimize}
              isLoading={isLoading}
              startingPoint={startingPoint}
              onStartingPointChange={setStartingPoint}
            />

            <div className="card">
              <button
                type="button"
                className="btn btn-secondary"
                onClick={handleReset}
                disabled={isLoading}
              >
                Reset
              </button>
            </div>
          </div>
          <div>
            <ResultsPanel result={result} isLoading={isLoading} error={error} />
          </div>
        </div>

        <MapView
          addresses={
            result?.optimizedAddresses || [
              startingPoint,
              ...addresses.map((a) => a.value),
            ]
          }
          polyline={result?.polyline}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
}

export default App;
