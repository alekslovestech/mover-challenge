import React, { useState, useEffect } from "react";
import { AddressItem } from "../types";

interface AddressFormProps {
  addresses: AddressItem[];
  onAddressesChange: (addresses: AddressItem[]) => void;
  onOptimize: () => void;
  isLoading: boolean;
}

const AddressForm: React.FC<AddressFormProps> = ({
  addresses,
  onAddressesChange,
  onOptimize,
  isLoading,
}) => {
  const [startingPoint, setStartingPoint] = useState("");

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
    onAddressesChange(
      addresses.map((addr) => (addr.id === id ? { ...addr, value } : addr))
    );
  };

  const canOptimize =
    addresses.length >= 2 &&
    addresses.every((addr) => addr.value.trim() !== "") &&
    !isLoading;

  // Initialize Google Places Autocomplete for address inputs using the new PlaceAutocompleteElement
  useEffect(() => {
    const initAutocomplete = () => {
      if (window.google?.maps?.places) {
        const inputs = document.querySelectorAll(
          'input[placeholder="Enter delivery address"]'
        );
        inputs.forEach((input) => {
          if (input && !input.getAttribute("data-autocomplete-initialized")) {
            // Create a new PlaceAutocompleteElement
            const autocompleteElement =
              new google.maps.places.PlaceAutocompleteElement({
                types: ["address"],
              });

            // Replace the original input with the autocomplete element
            const parent = input.parentNode;
            if (parent) {
              parent.replaceChild(autocompleteElement, input);

              // Add event listener for place selection
              autocompleteElement.addEventListener(
                "gmp-placeselect",
                (event: any) => {
                  const place = event.place;
                  if (place?.formattedAddress) {
                    // Update the React state
                    const addressId = (input as HTMLInputElement).getAttribute(
                      "data-address-id"
                    );
                    if (addressId) {
                      updateAddress(addressId, place.formattedAddress);
                    }
                  }
                }
              );

              // Store the address ID for React state updates
              autocompleteElement.setAttribute(
                "data-address-id",
                (input as HTMLInputElement).getAttribute("data-address-id") ||
                  ""
              );

              input.setAttribute("data-autocomplete-initialized", "true");
            }
          }
        });
      }
    };

    // Wait for Google Maps to load
    const timer = setTimeout(initAutocomplete, 1000);
    return () => clearTimeout(timer);
  }, [addresses]);

  return (
    <div className="card">
      <h2>Route Optimization</h2>

      <div className="form-group">
        <label htmlFor="startingPoint">Starting Point (Optional)</label>
        <input
          id="startingPoint"
          type="text"
          value={startingPoint}
          onChange={(e) => setStartingPoint(e.target.value)}
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

      {addresses.length < 2 && (
        <p style={{ color: "#666", fontSize: "14px", marginTop: "10px" }}>
          Add at least 2 addresses to optimize the route.
        </p>
      )}
    </div>
  );
};

export default AddressForm;
