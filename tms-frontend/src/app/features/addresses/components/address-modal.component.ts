import { Component, Input, input, output, OnInit, signal, computed, inject } from '@angular/core';
import { AddressDto } from '../../../core/services/address.service';
import { CityDto } from '../../../core/services/city.service';
import { AutocompleteComponent } from '../../../shared/components/autocomplete.component';
import { ToastService } from '../../../shared/services/toast.service';

export interface SaveAddressRequest {
  street: string;
  houseNumber: string;
  supplement?: string;
  cityName: string;
  zipCode: string;
  countryName: string;
}

interface FieldErrors {
  streetErr: string | null;
  houseNumberErr: string | null;
  supplementErr: string | null;
  cityErr: string | null;
  zipErr: string | null;
  countryErr: string | null;
}

@Component({
  selector: 'app-address-modal',
  standalone: true,
  imports: [AutocompleteComponent],
  template: `
    <div class="overlay" (click)="onOverlayClick($event)">
      <div class="modal">
        <h3>{{ address() ? 'Adresse bearbeiten' : 'Neue Adresse' }}</h3>

        <div class="form-grid">
          <div class="form-field">
            <label>Straße</label>
            <app-autocomplete
              [value]="street()"
              [suggestions]="streets()"
              placeholder="z. B. Musterstraße"
              [invalid]="!!fieldErrors().streetErr"
              (valueChange)="street.set($event)" />
            @if (fieldErrors().streetErr) {
              <span class="error-msg">{{ fieldErrors().streetErr }}</span>
            }
            @if (serverErrors()['Street']?.[0]) {
              <span class="error-msg">{{ serverErrors()['Street'][0] }}</span>
            }
          </div>

          <div class="form-field">
            <label>Hausnummer</label>
            <input
              [value]="houseNumber()"
              placeholder="12a"
              [class.invalid]="!!fieldErrors().houseNumberErr"
              (input)="houseNumber.set(getValue($event))" />
            @if (fieldErrors().houseNumberErr) {
              <span class="error-msg">{{ fieldErrors().houseNumberErr }}</span>
            }
            @if (serverErrors()['HouseNumber']?.[0]) {
              <span class="error-msg">{{ serverErrors()['HouseNumber'][0] }}</span>
            }
          </div>

          <div class="form-field full">
            <label>Zusatz <span class="optional">(optional)</span></label>
            <input
              [value]="supplement()"
              placeholder="z. B. 2. OG, Hinterhaus"
              [class.invalid]="!!fieldErrors().supplementErr"
              (input)="supplement.set(getValue($event))" />
            @if (fieldErrors().supplementErr) {
              <span class="error-msg">{{ fieldErrors().supplementErr }}</span>
            }
          </div>

          <div class="form-field">
            <label>Stadt</label>
            <app-autocomplete
              [value]="cityName()"
              [suggestions]="citySuggestions()"
              placeholder="z. B. Berlin"
              [invalid]="!!fieldErrors().cityErr"
              (valueChange)="onCitySelect($event)" />
            @if (fieldErrors().cityErr) {
              <span class="error-msg">{{ fieldErrors().cityErr }}</span>
            }
            @if (serverErrors()['CityName']?.[0]) {
              <span class="error-msg">{{ serverErrors()['CityName'][0] }}</span>
            }
          </div>

          <div class="form-field">
            <label>PLZ</label>
            <app-autocomplete
              [value]="zipCode()"
              [suggestions]="zipSuggestions()"
              placeholder="10115"
              [invalid]="!!fieldErrors().zipErr"
              (valueChange)="zipCode.set($event)" />
            @if (fieldErrors().zipErr) {
              <span class="error-msg">{{ fieldErrors().zipErr }}</span>
            }
            @if (serverErrors()['ZipCode']?.[0]) {
              <span class="error-msg">{{ serverErrors()['ZipCode'][0] }}</span>
            }
          </div>

          <div class="form-field full">
            <label>Land</label>
            <app-autocomplete
              [value]="countryName()"
              [suggestions]="countrySuggestions()"
              placeholder="z. B. Deutschland"
              [invalid]="!!fieldErrors().countryErr"
              (valueChange)="countryName.set($event)" />
            @if (fieldErrors().countryErr) {
              <span class="error-msg">{{ fieldErrors().countryErr }}</span>
            }
            @if (serverErrors()['CountryName']?.[0]) {
              <span class="error-msg">{{ serverErrors()['CountryName'][0] }}</span>
            }
          </div>

          @if (serverErrors()['general']?.[0]) {
            <div class="full">
              <p class="server-error">{{ serverErrors()['general'][0] }}</p>
            </div>
          }
        </div>

        <div class="modal-actions">
          @if (address()) {
            <button type="button" class="btn-danger" (click)="onDelete()">Löschen</button>
          }
          <div class="right-actions">
            <button type="button" class="btn-secondary" (click)="cancel.emit()">Abbrechen</button>
            <button type="button" class="btn-primary" (click)="onSave()">Speichern</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .overlay {
      position: fixed;
      inset: 0;
      z-index: 100;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 16px;
      background: rgba(17, 29, 44, 0.55);
    }
    .modal {
      width: 500px;
      max-width: 100%;
      padding: 36px;
      background: var(--color-white);
      border-radius: 12px;
      box-shadow: 0 16px 48px rgba(17, 29, 44, 0.3);

      h3 {
        margin-bottom: 24px;
        font-size: 18px;
        font-weight: 700;
        color: var(--color-steel);
      }
    }
    .form-grid {
      display: grid;
      grid-template-columns: 1fr 112px;
      gap: 16px 12px;
      margin-bottom: 28px;

      .full { grid-column: 1 / -1; }
    }
    .form-field {
      display: flex;
      flex-direction: column;
      gap: 6px;

      label { font-size: 13px; font-weight: 600; }
    }
    .optional { font-weight: 400; color: var(--color-text-muted); }
    .error-msg { font-size: 12px; color: var(--color-danger); }
    .server-error {
      padding: 8px 10px;
      font-size: 12px;
      color: var(--color-danger);
      background: #fdf2f2;
      border: 1px solid #f5c6c6;
      border-radius: var(--radius);
    }
    .modal-actions {
      display: flex;
      align-items: center;
      justify-content: space-between;

      button { height: 38px; padding: 0 20px; }
    }
    .right-actions {
      display: flex;
      gap: 8px;
      margin-left: auto;
    }

    @media (max-width: 480px) {
      .modal {
        max-height: 92vh;
        padding: 24px 20px;
        overflow-y: auto;
      }
      .form-grid { grid-template-columns: 1fr 92px; }
      .modal-actions {
        flex-direction: column-reverse;
        align-items: stretch;
        gap: 10px;
      }
      .right-actions {
        flex-direction: column-reverse;
        margin-left: 0;
      }
    }
  `]
})
export class AddressModalComponent implements OnInit {
  address = input<AddressDto | null>(null);
  cities = input<CityDto[]>([]);
  streets = input<string[]>([]);
  serverErrors = input<Record<string, string[]>>({});

  save = output<SaveAddressRequest>();
  delete = output<string>();
  cancel = output<void>();

  private toast = inject(ToastService);

  street = signal('');
  houseNumber = signal('');
  supplement = signal('');
  cityName = signal('');
  zipCode = signal('');
  countryName = signal('');

  fieldErrors = signal<FieldErrors>({
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
      this.street.set(a.street);
      this.houseNumber.set(a.houseNumber);
      this.supplement.set(a.supplement ?? '');
      this.cityName.set(a.cityName);
      this.zipCode.set(a.zipCode);
      this.countryName.set(a.countryName);
    }
  }

  onOverlayClick(event: MouseEvent) {
    if (event.target === event.currentTarget) {
      this.cancel.emit();
    }
  }

  onCitySelect(value: string) {
    this.cityName.set(value);
    const match = this.cities().find(c =>
      c.name.toLowerCase() === value.toLowerCase()
    );
    if (match) {
      this.zipCode.set(match.zipCode);
      this.countryName.set(match.countryName);
    }
  }

  getValue(event: Event): string {
    return (event.target as HTMLInputElement).value;
  }

  onSave() {
    const streetErr = !this.street().trim() ? 'Pflichtfeld'
      : this.street().length > 200 ? 'Maximal 200 Zeichen' : null;

    const houseNumberErr = !this.houseNumber().trim() ? 'Pflichtfeld'
      : this.houseNumber().length > 10 ? 'Maximal 10 Zeichen' : null;

    const supplementErr = this.supplement().length > 100
      ? 'Maximal 100 Zeichen' : null;

    const cityErr = !this.cityName().trim() ? 'Pflichtfeld'
      : this.cityName().length > 100 ? 'Maximal 100 Zeichen' : null;

    const zipErr = !this.zipCode().trim() ? 'Pflichtfeld'
      : this.zipCode().length > 20 ? 'Maximal 20 Zeichen' : null;

    const countryErr = !this.countryName().trim() ? 'Pflichtfeld'
      : this.countryName().length > 100 ? 'Maximal 100 Zeichen' : null;

    this.fieldErrors.set({
      streetErr, houseNumberErr, supplementErr,
      cityErr, zipErr, countryErr
    });

    const hasErrors = [streetErr, houseNumberErr, supplementErr,
      cityErr, zipErr, countryErr].some(e => e !== null);

    if (hasErrors) {
      this.toast.show('Bitte alle Pflichtfelder prüfen.', 'warning');
      return;
    }

    this.save.emit({
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