export interface RouteRequest {
  addresses: string[];
}

export interface RouteResponse {
  optimizedAddresses: string[];
  totalDistance: number;
  totalDuration: number;
  errorMessage?: string;
}

export interface AddressItem {
  id: string;
  value: string;
}
