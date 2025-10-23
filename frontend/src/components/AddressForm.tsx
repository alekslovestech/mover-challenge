import React, { useEffect, useMemo, useRef } from "react";
import { AddressItem } from "../types";

interface AddressFormProps {
  addresses: AddressItem[];
  onAddressesChange: (addresses: AddressItem[]) => void;
  onOptimize: () => void;
  isLoading: boolean;
  startingPoint: string;
  onStartingPointChange: (value: string) => void;
}

const AddressForm: React.FC<AddressFormProps> = ({
  addresses,
  onAddressesChange,
  onOptimize,
  isLoading,
  startingPoint,
  onStartingPointChange,
}) => {
  // Use a ref to store the latest addresses to avoid stale closure issues
  const addressesRef = useRef(addresses);
  addressesRef.current = addresses;

  const addAddress = () => {
    const newAddress: AddressItem = {
      id: Date.now().toString(),
      value: "",
    };
    onAddressesChange([...addresses, newAddress]);
  };

  const removeAddress = (id: string) => {
    onAddressesChange(addresses.filter((addr) => addr.id !== id));
  };

  const updateAddress = (id: string, value: string) => {
    // Use the ref to get the latest addresses instead of the stale closure
    const currentAddresses = addressesRef.current;
    const updatedAddresses = currentAddresses.map((addr) =>
      addr.id === id ? { ...addr, value } : addr
    );
    onAddressesChange(updatedAddresses);
  };

  const canOptimize = useMemo(() => {
    const validAddresses = addresses.filter((addr) => addr.value.trim() !== "");
    return validAddresses.length >= 2 && !isLoading;
  }, [addresses, isLoading]);

  // Initialize Google Places Autocomplete for address inputs using the legacy API
  useEffect(() => {
    const initAutocomplete = () => {
      if (window.google?.maps?.places) {
        const inputs = document.querySelectorAll(
          'input[placeholder="Enter delivery address"], input[placeholder="Enter starting address (leave empty to use first address)"]'
        );

        inputs.forEach((input) => {
          if (!input.getAttribute("data-autocomplete-initialized")) {
            const autocomplete = new google.maps.places.Autocomplete(
              input as HTMLInputElement,
              {
                types: ["address"],
              }
            );

            // Only update when user explicitly selects from dropdown
            autocomplete.addListener("place_changed", () => {
              const place = autocomplete.getPlace();
              // Only update if user selected a complete place with all required fields
              if (
                place?.formatted_address &&
                place.place_id &&
                place.geometry &&
                place.geometry.location
              ) {
                const address = place.formatted_address;

                // Check if this is the starting point input
                const isStartingPoint = input.id === "startingPoint";

                if (isStartingPoint) {
                  onStartingPointChange(address);
                } else {
                  const addressId = input.getAttribute("data-address-id");
                  if (addressId) {
                    updateAddress(addressId, address);
                  } else {
                    console.error("No address ID found for input:", input);
                  }
                }
              }
            });

            input.setAttribute("data-autocomplete-initialized", "true");
          }
        });
      }
    };

    const timer = setTimeout(initAutocomplete, 1000);
    return () => clearTimeout(timer);
  }, [addresses, onStartingPointChange, updateAddress]);

  return (
    <div className="card">
      <h2>Route Optimization</h2>

      <div className="form-group">
        <label htmlFor="startingPoint">Starting Point (Optional)</label>
        <input
          id="startingPoint"
          type="text"
          value={startingPoint}
          onChange={(e) => onStartingPointChange(e.target.value)}
          placeholder="Enter starting address (leave empty to use first address)"
        />
      </div>

      <div className="form-group">
        <label>Delivery Addresses</label>
        {addresses.map((address) => (
          <div key={address.id} className="address-item">
            <input
              type="text"
              value={address.value}
              onChange={(e) => updateAddress(address.id, e.target.value)}
              placeholder="Enter delivery address"
              data-address-id={address.id}
            />
            <button
              type="button"
              className="btn btn-danger"
              onClick={() => removeAddress(address.id)}
              disabled={addresses.length <= 2}
            >
              Remove
            </button>
          </div>
        ))}

        <button
          type="button"
          className="btn btn-secondary"
          onClick={addAddress}
        >
          Add Address
        </button>
      </div>

      <button
        type="button"
        className="btn"
        onClick={onOptimize}
        disabled={!canOptimize}
      >
        {isLoading ? "Optimizing..." : "Optimize Route"}
      </button>
      <div
        id="nearest-recharging-station"
        className="form-group"
        style={{ marginLeft: "10px" }}
      >
        {/*<label style={{ maxWidth: "200px", display: "flex", flexWrap: "wrap" }}>
          <input
            type="checkbox"
            defaultChecked={false}
            onChange={(e) =>
              console.log(
                "Include recharging station clicked",
                e.target.checked
              )
            }
          />
          {" Route via nearest recharging station"}
        </label>*/}
      </div>

      {addresses.length < 2 && (
        <p style={{ color: "#666", fontSize: "14px", marginTop: "10px" }}>
          Add at least 2 addresses to optimize the route.
        </p>
      )}
    </div>
  );
};

export default AddressForm;
