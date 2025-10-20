import axios from "axios";
import { RouteRequest, RouteResponse } from "../types";

const API_BASE_URL = process.env.REACT_APP_API_URL || "http://localhost:5000";

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

export const routeApi = {
  optimizeRoute: async (request: RouteRequest): Promise<RouteResponse> => {
    const response = await api.post<RouteResponse>(
      "/api/route/optimize",
      request
    );
    return response.data;
  },

  healthCheck: async (): Promise<{ status: string; timestamp: string }> => {
    const response = await api.get("/api/route/health");
    return response.data;
  },
};

export default api;
