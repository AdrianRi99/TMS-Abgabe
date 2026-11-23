import { Component, OnInit, computed, input, output, signal } from '@angular/core';
import { AddressDto } from '../../addresses/data/address.models';
import { formatDate, formatPrice } from '../data/order-format';
import {
  ORDER_STATUSES, OrderDto, OrderItemRequest, OrderStatus
} from '../data/order.models';
import { AddressPickerComponent } from './address-picker.component';

export interface SaveOrderRequest {
  /** Nur beim Anlegen gesetzt. */
  addressId?: string;
  /** Nur beim Bearbeiten gesetzt. */
  status?: OrderStatus;
  items: OrderItemRequest[];
}

interface ItemRow {
  key: number;
  description: string;
  price: number | null;
  quantity: number | null;
}

interface ItemErrors {
  description: string | null;
  price: string | null;
  quantity: string | null;
}

const EMPTY_ITEM_ERRORS: ItemErrors = { description: null, price: null, quantity: null };

@Component({
  selector: 'app-order-modal',
  standalone: true,
  imports: [AddressPickerComponent],
  templateUrl: './order-modal.component.html',
  styleUrl: './order-modal.component.scss'
})
export class OrderModalComponent implements OnInit {
  order = input<OrderDto | null>(null);
  serverErrors = input<Record<string, string[]>>({});

  save = output<SaveOrderRequest>();
  delete = output<string>();
  cancel = output<void>();

  readonly statuses = ORDER_STATUSES;
  readonly formatPrice = formatPrice;
  readonly formatDate = formatDate;

  address = signal<AddressDto | null>(null);
  status = signal<OrderStatus>('Offen');
  rows = signal<ItemRow[]>([]);

  addressErr = signal<string | null>(null);
  itemsErr = signal<string | null>(null);
  rowErrors = signal<Record<number, ItemErrors>>({});
  confirmingDelete = signal(false);

  private nextKey = 0;

  total = computed(() =>
    this.rows().reduce((sum, r) => sum + (r.price ?? 0) * (r.quantity ?? 0), 0)
  );

  ngOnInit() {
    const o = this.order();
    if (o) {
      this.status.set(o.status);
      this.rows.set(o.items.map(i => this.newRow(i.description, i.price, i.quantity)));
    } else {
      this.rows.set([this.newRow()]);
    }
  }

  // --- Positionen ---
  private newRow(description = '', price: number | null = null, quantity: number | null = 1): ItemRow {
    return { key: this.nextKey++, description, price, quantity };
  }

  addRow() {
    this.rows.update(rows => [...rows, this.newRow()]);
  }

  removeRow(key: number) {
    this.rows.update(rows => rows.filter(r => r.key !== key));
  }

  updateRow(key: number, patch: Partial<Omit<ItemRow, 'key'>>) {
    this.rows.update(rows => rows.map(r => r.key === key ? { ...r, ...patch } : r));
  }

  inputValue(event: Event): string {
    return (event.target as HTMLInputElement).value;
  }

  onStatusChange(event: Event) {
    const value = this.inputValue(event);
    const match = ORDER_STATUSES.find(s => s === value);
    if (match) this.status.set(match);
  }

  /** Leeres oder ungültiges Zahlenfeld -> null. */
  numberValue(event: Event): number | null {
    const el = event.target as HTMLInputElement;
    return el.value === '' || Number.isNaN(el.valueAsNumber) ? null : el.valueAsNumber;
  }

  errorsFor(key: number): ItemErrors {
    return this.rowErrors()[key] ?? EMPTY_ITEM_ERRORS;
  }

  // --- Server-Fehler (FluentValidation: "Items[0].Description" usw.) ---
  serverError(key: string): string | null {
    return this.serverErrors()[key]?.[0] ?? null;
  }

  itemServerError(index: number, field: 'Description' | 'Price' | 'Quantity'): string | null {
    return this.serverError(`Items[${index}].${field}`);
  }

  // --- Aktionen ---
  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.cancel.emit();
    }
  }

  onSave() {
    const isEdit = !!this.order();

    const addressErr = !isEdit && !this.address() ? 'Bitte eine Adresse auswählen' : null;
    const rows = this.rows();
    const itemsErr = rows.length === 0 ? 'Mindestens eine Position erforderlich' : null;

    const errors: Record<number, ItemErrors> = {};
    for (const r of rows) {
      errors[r.key] = {
        description: !r.description.trim() ? 'Pflichtfeld'
          : r.description.length > 500 ? 'Maximal 500 Zeichen' : null,
        price: r.price === null ? 'Pflichtfeld'
          : r.price < 0 ? 'Mindestens 0' : null,
        quantity: r.quantity === null ? 'Pflichtfeld'
          : !Number.isInteger(r.quantity) || r.quantity < 1 ? 'Mindestens 1' : null
      };
    }

    this.addressErr.set(addressErr);
    this.itemsErr.set(itemsErr);
    this.rowErrors.set(errors);

    const hasRowErrors = Object.values(errors).some(e => e.description || e.price || e.quantity);
    if (addressErr || itemsErr || hasRowErrors) return;

    const items: OrderItemRequest[] = rows.map(r => ({
      description: r.description.trim(),
      price: r.price as number,
      quantity: r.quantity as number
    }));

    this.save.emit(isEdit
      ? { status: this.status(), items }
      : { addressId: this.address()!.id, items });
  }

  onDelete() {
    const o = this.order();
    if (!o) return;
    if (!this.confirmingDelete()) {
      this.confirmingDelete.set(true);
      return;
    }
    this.delete.emit(o.id);
  }
}
