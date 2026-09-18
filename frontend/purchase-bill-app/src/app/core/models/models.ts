export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserLocation {
  location_Code: string;
  location_Name: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  token?: string;
  userLocations: UserLocation[];
}

export interface Location {
  location_Code: string;
  location_Name: string;
}

export interface PurchaseBillItem {
  id?: number;
  itemName: string;
  batch: string;
  standardCost: number;
  standardPrice: number;
  margin: number;
  qty: number;
  freeQty: number;
  discount: number;
  totalCost: number;
  totalSelling: number;
}