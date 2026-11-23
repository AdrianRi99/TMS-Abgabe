import { Component, OnInit, inject } from '@angular/core';
import { OrdersPageFacade } from './data/orders-page.facade';
import { OrderStatus } from './data/order.models';
import { formatDate, formatPrice } from './data/order-format';
import { OrderModalComponent } from './components/order-modal.component';

@Component({
  selector: 'app-orders-page',
  standalone: true,
  imports: [OrderModalComponent],
  providers: [OrdersPageFacade],
  templateUrl: './orders-page.component.html',
  styleUrl: './orders-page.component.scss'
})
export class OrdersPageComponent implements OnInit {
  vm = inject(OrdersPageFacade);

  readonly formatPrice = formatPrice;
  readonly formatDate = formatDate;

  ngOnInit() {
    this.vm.init();
  }

  statusClass(status: OrderStatus): string {
    switch (status) {
      case 'Abgeschlossen': return 'status-done';
      case 'Storniert': return 'status-cancelled';
      default: return 'status-open';
    }
  }
}
