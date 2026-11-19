import { Component, input, output, OnInit, signal, computed } from '@angular/core';
import { AddressDto } from '../data/address.models';
import { CityDto } from '../data/city.models';
import { AutocompleteComponent } from '../../../shared/components/autocomplete.component';

export interface SaveAddressRequest {
  name: string;
  street: string;
  houseNumber: string;
  supplement?: string;
  cityName: string;
  zipCode: string;
  countryName: string;
}

@Component({
  selector: 'app-address-modal',
  standalone: true,
  imports: [AutocompleteComponent],
  templateUrl: './address-modal.component.html',
  styleUrl: './address-modal.component.scss'
})
export class AddressModalComponent implements OnInit {
  address = input<AddressDto | null>(null);
  cities = input<CityDto[]>([]);
  streets = input<string[]>([]);
  serverErrors = input<Record<string, string[]>>({});

  save = output<SaveAddressRequest>();
  delete = output<string>();
  cancel = output<void>();

  name = signal('');
  street = signal('');
  houseNumber = signal('');
  supplement = signal('');
  cityName = signal('');
  zipCode = signal('');
  countryName = signal('');

  fieldErrors = signal<{
    nameErr: string | null;
    streetErr: string | null;
    houseNumberErr: string | null;
    supplementErr: string | null;
    cityErr: string | null;
    zipErr: string | null;
    countryErr: string | null;
  }>({
    nameErr: null,
    streetErr: null,
    houseNumberErr: null,
    supplementErr: null,
    cityErr: null,
    zipErr: null,
    countryErr: null
  });

  citySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.name))].sort()
  );
  zipSuggestions = computed(() => {
    const name = this.cityName().toLowerCase();
    return [...new Set(
      this.cities()
        .filter(c => !name || c.name.toLowerCase().includes(name))
        .map(c => c.zipCode)
    )].sort();
  });
  countrySuggestions = computed(() =>
    [...new Set(this.cities().map(c => c.countryName))].sort()
  );

  ngOnInit() {
    const a = this.address();
    if (a) {
      this.name.set(a.name ?? '');
      this.street.set(a.street);
      this.houseNumber.set(a.houseNumber);
      this.supplement.set(a.supplement ?? '');
      this.cityName.set(a.cityName);
      this.zipCode.set(a.zipCode);
      this.countryName.set(a.countryName);
    }
  }

  onCitySelect(value: string) {
    this.cityName.set(value);
    const match = this.cities().find(c => c.name.toLowerCase() === value.toLowerCase());
    if (match) {
      this.zipCode.set(match.zipCode);
      this.countryName.set(match.countryName);
    }
  }

  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.cancel.emit();
    }
  }

  getValue(event: Event): string {
    return (event.target as HTMLInputElement).value;
  }

  onSave() {
    const nameErr = !this.name().trim() ? 'Pflichtfeld'
      : this.name().length > 200 ? 'Maximal 200 Zeichen' : null;

    const streetErr = !this.street().trim() ? 'Pflichtfeld'
      : this.street().length > 200 ? 'Maximal 200 Zeichen' : null;

    const houseNumberErr = !this.houseNumber().trim() ? 'Pflichtfeld'
      : this.houseNumber().length > 10 ? 'Maximal 10 Zeichen' : null;

    const supplementErr = this.supplement().length > 100 ? 'Maximal 100 Zeichen' : null;

    const cityErr = !this.cityName().trim() ? 'Pflichtfeld'
      : this.cityName().length > 100 ? 'Maximal 100 Zeichen' : null;

    const zipErr = !this.zipCode().trim() ? 'Pflichtfeld'
      : this.zipCode().length > 20 ? 'Maximal 20 Zeichen' : null;

    const countryErr = !this.countryName().trim() ? 'Pflichtfeld'
      : this.countryName().length > 100 ? 'Maximal 100 Zeichen' : null;

    this.fieldErrors.set({ nameErr, streetErr, houseNumberErr, supplementErr, cityErr, zipErr, countryErr });

    const hasErrors = [nameErr, streetErr, houseNumberErr, supplementErr, cityErr, zipErr, countryErr]
      .some(e => e !== null);

    if (hasErrors) return;

    this.save.emit({
      name: this.name().trim(),
      street: this.street().trim(),
      houseNumber: this.houseNumber().trim(),
      supplement: this.supplement().trim() || undefined,
      cityName: this.cityName().trim(),
      zipCode: this.zipCode().trim(),
      countryName: this.countryName().trim()
    });
  }

  onDelete() {
    const a = this.address();
    if (a) this.delete.emit(a.id);
  }
}