export const ORDER_STATUSES = ['Offen', 'Abgeschlossen', 'Storniert'] as const;
export type OrderStatus = typeof ORDER_STATUSES[number];

export interface OrderItemDto {
  id: string;
  description: string;
  price: number;
  quantity: number;
}

export interface OrderDto {
  id: string;
  addressId: string;
  customerName: string;
  orderDate: string;
  status: OrderStatus;
  items: OrderItemDto[];
  totalPrice: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface OrderItemRequest {
  description: string;
  price: number;
  quantity: number;
}

export interface CreateOrderRequest {
  addressId: string;
  items: OrderItemRequest[];
}

export interface UpdateOrderRequest {
  status: OrderStatus;
  items: OrderItemRequest[];
}

export interface OrderSearchParams {
  page?: number;
  pageSize?: number;
  sortBy?: 'orderdate' | 'customername' | 'status';
  sortDirection?: 'asc' | 'desc';
}
