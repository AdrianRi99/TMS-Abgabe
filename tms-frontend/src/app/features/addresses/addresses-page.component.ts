import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AddressService, AddressDto } from '../../core/services/address.service';
import { CityService, CityDto } from '../../core/services/city.service';
import { AutocompleteComponent } from '../../shared/components/autocomplete.component';
import { AddressModalComponent, SaveAddressRequest } from './components/address-modal.component';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-addresses-page',
  standalone: true,
  imports: [AutocompleteComponent, AddressModalComponent],
  template: `
    <div class="page">
      <section class="filter-card" (keydown.enter)="search()">
        <div class="filter-field">
          <label>Straße</label>
          <app-autocomplete
            [value]="filterStreet()"
            [suggestions]="streetSuggestions()"
            placeholder="z. B. Musterstraße"
            (valueChange)="filterStreet.set($event)" />
        </div>
        <div class="filter-field">
          <label>Stadt</label>
          <app-autocomplete
            [value]="filterCity()"
            [suggestions]="citySuggestions()"
            placeholder="z. B. Berlin"
            (valueChange)="filterCity.set($event)" />
        </div>
        <div class="filter-field">
          <label>Land</label>
          <app-autocomplete
            [value]="filterCountry()"
            [suggestions]="countrySuggestions()"
            placeholder="z. B. Deutschland"
            (valueChange)="filterCountry.set($event)" />
        </div>
        <div class="filter-actions">
          <button type="button" class="btn-primary" (click)="search()">Suchen</button>
          <button type="button" class="reset-link" (click)="resetFilter()">Filter zurücksetzen</button>
        </div>
      </section>

      <div class="page-header">
        <h2>Adressen</h2>
        <button type="button" class="btn-primary new-btn" (click)="openCreate()">
          <span class="label-full">+ Neue Adresse</span>
          <span class="label-short">+</span>
        </button>
      </div>

      <div class="list">
        <div class="table-card">
          <table>
            <colgroup>
              <col style="width: 20%" />
              <col style="width: 20%" />
              <col style="width: 16%" />
              <col style="width: 12%" />
              <col style="width: 14%" />
              <col style="width: 18%" />
            </colgroup>
            <thead>
              <tr>
                <th (click)="sort('street')" class="sortable">
                  STRASSE <span class="sort-icon">{{ sortIcons()['street'] }}</span>
                </th>
                <th (click)="sort('housenumber')" class="sortable">
                  HAUSNUMMER <span class="sort-icon">{{ sortIcons()['housenumber'] }}</span>
                </th>
                <th>ZUSATZ</th>
                <th (click)="sort('zipcode')" class="sortable">
                  PLZ <span class="sort-icon">{{ sortIcons()['zipcode'] }}</span>
                </th>
                <th (click)="sort('city')" class="sortable">
                  STADT <span class="sort-icon">{{ sortIcons()['city'] }}</span>
                </th>
                <th (click)="sort('country')" class="sortable">
                  LAND <span class="sort-icon">{{ sortIcons()['country'] }}</span>
                </th>
              </tr>
            </thead>
            <tbody>
              @for (address of addresses(); track address.id) {
                <tr (click)="openEdit(address)">
                  <td>{{ address.street }}</td>
                  <td>{{ address.houseNumber }}</td>
                  <td class="muted">{{ address.supplement ?? '–' }}</td>
                  <td>{{ address.zipCode }}</td>
                  <td>{{ address.cityName }}</td>
                  <td>{{ address.countryName }}</td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty">Keine Adressen gefunden.</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="cards">
          @for (address of addresses(); track address.id) {
            <div class="address-card" tabindex="0" role="button"
                 (click)="openEdit(address)"
                 (keydown.enter)="openEdit(address)">
              <div class="title">
                {{ address.street }} {{ address.houseNumber }}{{ address.supplement ? ', ' + address.supplement : '' }}
              </div>
              <div class="sub">
                {{ address.zipCode }} {{ address.cityName }}, {{ address.countryName }}
              </div>
              <svg class="chevron" width="18" height="18" viewBox="0 0 24 24" fill="none"
                   stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M9 6l6 6-6 6" />
              </svg>
            </div>
          } @empty {
            <p class="empty-text">Keine Adressen gefunden.</p>
          }
        </div>

        <div class="pagination">
          <span>Seite {{ currentPage() }} von {{ totalPages() }}</span>
          <button type="button" class="btn-outline"
            [disabled]="currentPage() <= 1"
            (click)="prevPage()">Zurück</button>
          <button type="button" class="btn-primary"
            [disabled]="currentPage() >= totalPages()"
            (click)="nextPage()">Vor</button>
        </div>
      </div>
    </div>

    @if (modalOpen()) {
      <app-address-modal
        [address]="selectedAddress()"
        [cities]="cities()"
        [streets]="streetSuggestions()"
        [serverErrors]="modalServerErrors()"
        (save)="onSave($event)"
        (delete)="onDelete($event)"
        (cancel)="closeModal()" />
    }
  `,
  styles: [`
    .page { display: flex; flex-direction: column; }

    .filter-card {
      order: 1;
      align-self: flex-start;
      display: flex;
      align-items: flex-end;
      gap: 16px;
      padding: 20px 24px;
      margin-bottom: 28px;
      background: var(--color-white);
      border: 1px solid var(--color-border);
      border-radius: 8px;
    }
    .filter-field {
      display: flex;
      flex-direction: column;
      gap: 6px;
      width: 200px;

      label { font-size: 12px; font-weight: 600; }
    }
    .filter-actions {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .reset-link {
      height: auto;
      padding: 0;
      background: none;
      border: none;
      font-size: 12px;
      font-weight: 500;
      color: var(--color-steel);
      text-decoration: underline;
      cursor: pointer;
    }

    .page-header {
      order: 2;
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 16px;

      h2 { font-size: 18px; font-weight: 700; color: var(--color-steel); }
    }
    .new-btn { height: 38px; padding: 0 18px; }
    .label-short { display: none; }

    .list { order: 3; }

    .table-card {
      background: var(--color-white);
      border: 1px solid var(--color-border);
      border-radius: 8px;
      overflow: hidden;
    }
    table {
      width: 100%;
      border-collapse: collapse;
      table-layout: fixed;
    }
    th {
      padding: 12px 16px;
      background: var(--color-table-head);
      text-align: left;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.06em;
      text-transform: uppercase;
      color: var(--color-steel);
    }
    th.sortable {
      cursor: pointer;
      user-select: none;
      white-space: nowrap;

      &:hover { color: var(--color-primary); }
    }
    .sort-icon {
      display: inline-block;
      margin-left: 4px;
      font-size: 10px;
      color: var(--color-text-muted);
    }
    td {
      padding: 13px 16px;
      border-top: 1px solid var(--color-border-light);
      font-size: 13px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    tbody tr {
      cursor: pointer;
      &:hover td { background: #f6f9fc; }
    }
    td.muted { color: #6b7f94; }
    td.empty {
      padding: 32px;
      text-align: center;
      color: var(--color-text-muted);
      white-space: normal;
    }

    .cards {
      display: none;
      flex-direction: column;
      gap: 10px;
    }
    .address-card {
      position: relative;
      padding: 14px 40px 14px 16px;
      background: var(--color-white);
      border: 1px solid var(--color-border);
      border-radius: 8px;
      cursor: pointer;

      &:hover { background: #f6f9fc; }
      .title { font-size: 14px; font-weight: 600; }
      .sub { margin-top: 4px; font-size: 13px; color: var(--color-text-muted); }
      .chevron {
        position: absolute;
        top: 50%;
        right: 14px;
        transform: translateY(-50%);
        color: #9ca3af;
      }
    }
    .empty-text {
      padding: 24px;
      text-align: center;
      color: var(--color-text-muted);
    }

    .pagination {
      display: flex;
      align-items: center;
      justify-content: flex-end;
      gap: 10px;
      margin-top: 16px;
      font-size: 12px;
      color: var(--color-text-muted);

      button { height: 32px; padding: 0 14px; }
    }

    @media (max-width: 768px) {
      .page-header { order: 1; }
      .filter-card {
        order: 2;
        align-self: stretch;
        flex-direction: column;
        align-items: stretch;
        gap: 14px;
        padding: 16px;
        margin-bottom: 16px;
      }
      .filter-field { width: 100%; }
      .filter-actions {
        flex-direction: column;
        align-items: stretch;
        gap: 12px;

        .btn-primary { height: 40px; }
        .reset-link { align-self: center; }
      }
      .new-btn {
        width: 36px;
        height: 36px;
        padding: 0;
        font-size: 20px;
      }
      .label-full { display: none; }
      .label-short { display: inline; }
      .table-card { display: none; }
      .cards { display: flex; }
    }
  `]
})
export class AddressesPageComponent implements OnInit {
  private addressService = inject(AddressService);
  private cityService = inject(CityService);
  private toast = inject(ToastService);

  addresses = signal<AddressDto[]>([]);
  cities = signal<CityDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  readonly pageSize = 12;
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  filterStreet = signal('');
  filterCity = signal('');
  filterCountry = signal('');

  sortBy = signal('street');
  sortDirection = signal<'asc' | 'desc'>('asc');

  sortIcons = computed(() => {
    const by = this.sortBy();
    const dir = this.sortDirection();
    const fields = ['street', 'housenumber', 'zipcode', 'city', 'country'];
    const result: Record<string, string> = {};
    for (const f of fields) {
      result[f] = by === f ? (dir === 'asc' ? '↑' : '↓') : '↕';
    }
    return result;
  });

  streetSuggestions = computed(() =>
    [...new Set(this.addresses().map(a => a.street))].sort()
  );
  citySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.name))].sort()
  );
  countrySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.countryName))].sort()
  );

  modalOpen = signal(false);
  selectedAddress = signal<AddressDto | null>(null);
  modalServerErrors = signal<Record<string, string[]>>({});

  ngOnInit() {
    this.loadAddresses();
    this.cityService.getAll().subscribe(c => this.cities.set(c));
  }

  loadAddresses() {
    this.addressService.search({
      street: this.filterStreet() || undefined,
      cityName: this.filterCity() || undefined,
      countryName: this.filterCountry() || undefined,
      page: this.currentPage(),
      pageSize: this.pageSize,
      sortBy: this.sortBy(),
      sortDirection: this.sortDirection()
    }).subscribe(r => {
      this.addresses.set(r.items);
      this.totalCount.set(r.totalCount);
    });
  }

  search() {
    this.currentPage.set(1);
    this.loadAddresses();
  }

  resetFilter() {
    this.filterStreet.set('');
    this.filterCity.set('');
    this.filterCountry.set('');
    this.search();
  }

  sort(field: string) {
    if (this.sortBy() === field) {
      this.sortDirection.update(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(field);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
    this.loadAddresses();
  }

  prevPage() {
    this.currentPage.update(p => p - 1);
    this.loadAddresses();
  }

  nextPage() {
    this.currentPage.update(p => p + 1);
    this.loadAddresses();
  }

  openCreate() {
    this.selectedAddress.set(null);
    this.modalOpen.set(true);
  }

  openEdit(address: AddressDto) {
    this.selectedAddress.set(address);
    this.modalOpen.set(true);
  }

  closeModal() {
    this.modalOpen.set(false);
    this.modalServerErrors.set({});
  }

  onSave(req: SaveAddressRequest) {
    this.modalServerErrors.set({});
    const selected = this.selectedAddress();
    const call = selected
      ? this.addressService.update(selected.id, req)
      : this.addressService.create(req);

    call.subscribe({
      next: () => {
        this.toast.show(selected ? 'Adresse gespeichert.' : 'Adresse angelegt.', 'success');
        this.closeModal();
        this.loadAddresses();
        this.cityService.getAll().subscribe(c => this.cities.set(c));
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 400 && err.error?.errors) {
          this.modalServerErrors.set(err.error.errors);
        } else {
          this.toast.show('Fehler beim Speichern.', 'error');
        }
      }
    });
  }

  onDelete(id: string) {
    this.addressService.delete(id).subscribe({
      next: () => {
        this.toast.show('Adresse gelöscht.', 'success');
        this.closeModal();
        this.loadAddresses();
      },
      error: () => this.toast.show('Fehler beim Löschen.', 'error')
    });
  }
}