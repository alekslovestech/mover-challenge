import React, { useState } from "react";
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
