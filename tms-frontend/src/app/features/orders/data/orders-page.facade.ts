import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { OrderService } from './order.service';
import { OrderDto } from './order.models';
import { SaveOrderRequest } from '../components/order-modal.component';
import { ToastService } from '../../../shared/services/toast.service';

const SORT_FIELDS = ['orderdate', 'customername', 'status'] as const;
export type OrderSortField = typeof SORT_FIELDS[number];

@Injectable()
export class OrdersPageFacade {
  private orderService = inject(OrderService);
  private toast = inject(ToastService);

  // --- State ---
  orders = signal<OrderDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  readonly pageSize = 20;

  sortBy = signal<OrderSortField>('orderdate');
  sortDirection = signal<'asc' | 'desc'>('desc');

  modalOpen = signal(false);
  selectedOrder = signal<OrderDto | null>(null);
  modalServerErrors = signal<Record<string, string[]>>({});

  // --- Derived state ---
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  sortIcons = computed(() => {
    const by = this.sortBy();
    const dir = this.sortDirection();
    const result: Record<string, string> = {};
    for (const f of SORT_FIELDS) {
      result[f] = by === f ? (dir === 'asc' ? '↑' : '↓') : '↕';
    }
    return result;
  });

  // --- Lifecycle ---
  init() {
    this.loadOrders();
  }

  // --- Data loading ---
  loadOrders() {
    this.orderService.search({
      page: this.currentPage(),
      pageSize: this.pageSize,
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection()
    }).subscribe({
      next: r => {
        this.orders.set(r.items);
        this.totalCount.set(r.totalCount);
      },
      error: () => this.toast.show('Fehler beim Laden der Bestellungen.', 'error')
    });
  }

  // --- Sort actions ---
  sort(field: OrderSortField) {
    if (this.sortBy() === field) {
      this.sortDirection.update(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(field);
      // Datum: neueste zuerst, Text-Spalten: A–Z
      this.sortDirection.set(field === 'orderdate' ? 'desc' : 'asc');
    }
    this.currentPage.set(1);
    this.loadOrders();
  }

  // --- Paging actions ---
  prevPage() {
    this.currentPage.update(p => p - 1);
    this.loadOrders();
  }

  nextPage() {
    this.currentPage.update(p => p + 1);
    this.loadOrders();
  }

  // --- Modal actions ---
  openCreate() {
    this.selectedOrder.set(null);
    this.modalOpen.set(true);
  }

  openEdit(order: OrderDto) {
    this.selectedOrder.set(order);
    this.modalOpen.set(true);
  }

  closeModal() {
    this.modalOpen.set(false);
    this.modalServerErrors.set({});
  }

  save(req: SaveOrderRequest) {
    this.modalServerErrors.set({});
    const selected = this.selectedOrder();

    const call = selected
      ? this.orderService.update(selected.id, { status: req.status ?? selected.status, items: req.items })
      : this.orderService.create({ addressId: req.addressId!, items: req.items });

    call.subscribe({
      next: () => {
        this.toast.show(selected ? 'Bestellung gespeichert.' : 'Bestellung angelegt.', 'success');
        this.closeModal();
        this.loadOrders();
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 400 && err.error?.errors) {
          this.modalServerErrors.set(err.error.errors);
        } else if (err.status === 404) {
          this.toast.show('Bestellung existiert nicht mehr.', 'error');
          this.closeModal();
          this.loadOrders();
        } else {
          this.toast.show('Fehler beim Speichern.', 'error');
        }
      }
    });
  }

  delete(id: string) {
    this.orderService.delete(id).subscribe({
      next: () => {
        this.toast.show('Bestellung gelöscht.', 'success');
        this.closeModal();
        // Letzte Seite durch das Löschen leer geworden -> eine Seite zurück
        if (this.orders().length === 1 && this.currentPage() > 1) {
          this.currentPage.update(p => p - 1);
        }
        this.loadOrders();
      },
      error: () => this.toast.show('Fehler beim Löschen.', 'error')
    });
  }
}
