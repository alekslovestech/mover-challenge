export interface RouteRequest {
  addresses: string[];
}

export interface RouteResponse {
  optimizedAddresses: string[];
  totalDistance: number;
  totalDuration: number;
  polyline?: string;
  errorMessage?: string;
}

export interface AddressItem {
  id: string;
  value: string;
}
