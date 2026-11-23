import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../core/api-config';
import {
  CreateOrderRequest,
  OrderDto,
  OrderSearchParams,
  PagedResult,
  UpdateOrderRequest
} from './order.models';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);

  search(params: OrderSearchParams): Observable<PagedResult<OrderDto>> {
    const httpParams = new HttpParams()
      .set('page', params.page ?? 1)
      .set('pageSize', params.pageSize ?? 20)
      .set('sortBy', params.sortBy ?? 'orderdate')
      .set('sortDirection', params.sortDirection ?? 'desc');
    return this.http.get<PagedResult<OrderDto>>(`${API_URL}/orders`, { params: httpParams });
  }

  getById(id: string): Observable<OrderDto> {
    return this.http.get<OrderDto>(`${API_URL}/orders/${id}`);
  }

  create(request: CreateOrderRequest): Observable<OrderDto> {
    return this.http.post<OrderDto>(`${API_URL}/orders`, request);
  }

  update(id: string, request: UpdateOrderRequest): Observable<OrderDto> {
    return this.http.put<OrderDto>(`${API_URL}/orders/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${API_URL}/orders/${id}`);
  }
}
