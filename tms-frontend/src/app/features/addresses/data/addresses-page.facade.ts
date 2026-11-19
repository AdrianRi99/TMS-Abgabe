import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AddressService } from './address.service';
import { CityService } from './city.service';
import { AddressDto } from './address.models';
import { CityDto } from './city.models';
import { SaveAddressRequest } from '../components/address-modal.component';
import { ToastService } from '../../../shared/services/toast.service';

const SORT_FIELDS = ['name', 'street', 'housenumber', 'zipcode', 'city', 'country'] as const;
type SortField = typeof SORT_FIELDS[number];

@Injectable()
export class AddressesPageFacade {
  private addressService = inject(AddressService);
  private cityService = inject(CityService);
  private toast = inject(ToastService);

  // --- State ---
  addresses = signal<AddressDto[]>([]);
  cities = signal<CityDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  readonly pageSize = 20;

  filterStreet = signal('');
  filterCity = signal('');
  filterCountry = signal('');

  sortBy = signal<SortField>('street');
  sortDirection = signal<'asc' | 'desc'>('asc');

  modalOpen = signal(false);
  selectedAddress = signal<AddressDto | null>(null);
  modalServerErrors = signal<Record<string, string[]>>({});

  // --- Derived state ---
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  streetSuggestions = computed(() =>
    [...new Set(this.addresses().map(a => a.street))].sort()
  );
  citySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.name))].sort()
  );
  countrySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.countryName))].sort()
  );

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
    this.loadAddresses();
    this.cityService.getAll().subscribe(c => this.cities.set(c));
  }

  // --- Data loading ---
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

  // --- Filter actions ---
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

  // --- Sort actions ---
  sort(field: SortField) {
    if (this.sortBy() === field) {
      this.sortDirection.update(d => d === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(field);
      this.sortDirection.set('asc');
    }
    this.currentPage.set(1);
    this.loadAddresses();
  }

  // --- Paging actions ---
  prevPage() {
    this.currentPage.update(p => p - 1);
    this.loadAddresses();
  }

  nextPage() {
    this.currentPage.update(p => p + 1);
    this.loadAddresses();
  }

  // --- Modal actions ---
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

  save(req: SaveAddressRequest) {
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

        const cityKnown = this.cities().some(
          c => c.name.toLowerCase() === req.cityName.toLowerCase()
        );
        if (!cityKnown) {
          this.cityService.getAll().subscribe(c => this.cities.set(c));
        }
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

  delete(id: string) {
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