export interface AddressDto {
  id: string;
  name: string;
  street: string;
  houseNumber: string;
  supplement?: string;
  cityId: number;
  cityName: string;
  zipCode: string;
  countryId: number;
  countryName: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AddressSearchParams {
  name?: string;
  street?: string;
  cityName?: string;
  countryName?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface CreateAddressRequest {
  name: string;
  street: string;
  houseNumber: string;
  supplement?: string;
  cityName: string;
  zipCode: string;
  countryName: string;
}