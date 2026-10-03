import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../api-config';


export interface CityDto {
  id: number;
  name: string;
  zipCode: string;
  countryId: number;
  countryName: string;
}

@Injectable({ providedIn: 'root' })
export class CityService {
  private http = inject(HttpClient);

  getAll(): Observable<CityDto[]> {
    return this.http.get<CityDto[]>(`${API_URL}/cities`);
  }
}