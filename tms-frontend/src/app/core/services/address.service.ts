import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../api-config';

export interface AddressDto {
  id: string;
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
  street?: string;
  cityName?: string;
  countryName?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface CreateAddressRequest {
  street: string;
  houseNumber: string;
  supplement?: string;
  cityName: string;
  zipCode: string;
  countryName: string;
}

@Injectable({ providedIn: 'root' })
export class AddressService {
  private http = inject(HttpClient);

  search(params: AddressSearchParams): Observable<PagedResult<AddressDto>> {
    let httpParams = new HttpParams();
    if (params.street) httpParams = httpParams.set('street', params.street);
    if (params.cityName) httpParams = httpParams.set('cityName', params.cityName);
    if (params.countryName) httpParams = httpParams.set('countryName', params.countryName);
    httpParams = httpParams.set('page', params.page ?? 1);
    httpParams = httpParams.set('pageSize', params.pageSize ?? 20);
    httpParams = httpParams.set('sortBy', params.sortBy ?? 'street');
    httpParams = httpParams.set('sortDirection', params.sortDirection ?? 'asc');
    return this.http.get<PagedResult<AddressDto>>(`${API_URL}/addresses`, { params: httpParams });
  }

  getById(id: string): Observable<AddressDto> {
    return this.http.get<AddressDto>(`${API_URL}/addresses/${id}`);
  }

  create(request: CreateAddressRequest): Observable<AddressDto> {
    return this.http.post<AddressDto>(`${API_URL}/addresses`, request);
  }

  update(id: string, request: CreateAddressRequest): Observable<AddressDto> {
    return this.http.put<AddressDto>(`${API_URL}/addresses/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${API_URL}/addresses/${id}`);
  }
}