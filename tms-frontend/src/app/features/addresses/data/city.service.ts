import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../core/api-config';
import { CityDto } from './city.models';

@Injectable({ providedIn: 'root' })
export class CityService {
  private http = inject(HttpClient);

  getAll(): Observable<CityDto[]> {
    return this.http.get<CityDto[]>(`${API_URL}/cities`);
  }
}