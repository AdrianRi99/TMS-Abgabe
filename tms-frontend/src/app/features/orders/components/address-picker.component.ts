import {
  Component, ElementRef, HostListener, OnDestroy, inject, input, output, signal
} from '@angular/core';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { AddressService } from '../../addresses/data/address.service';
import { AddressDto } from '../../addresses/data/address.models';

export function addressLabel(a: AddressDto): string {
  return `${a.name} – ${a.street} ${a.houseNumber}, ${a.zipCode} ${a.cityName}`;
}

/** Sucht Adressen serverseitig nach Name und liefert die ausgewählte Adresse zurück. */
@Component({
  selector: 'app-address-picker',
  standalone: true,
  template: `
    @if (selected(); as address) {
      <div class="selected" [class.invalid]="invalid()">
        <span class="label">{{ label(address) }}</span>
        <button type="button" class="change" (click)="clear()">Ändern</button>
      </div>
    } @else {
      <div class="wrapper">
        <input
          [value]="term()"
          placeholder="Name der Adresse suchen …"
          [class.invalid]="invalid()"
          (input)="onInput($event)"
          (focus)="onFocus()"
          autocomplete="off" />
        @if (open()) {
          <ul class="dropdown" role="listbox">
            @for (a of results(); track a.id) {
              <li role="option" (mousedown)="select(a)">{{ label(a) }}</li>
            } @empty {
              <li class="hint">{{ term().trim() ? 'Keine Adresse gefunden.' : 'Keine Adressen vorhanden.' }}</li>
            }
          </ul>
        }
      </div>
    }
  `,
  styles: [`
    .wrapper { position: relative; }
    .dropdown {
      position: absolute;
      top: 100%;
      left: 0;
      right: 0;
      z-index: 50;
      max-height: 220px;
      overflow-y: auto;
      list-style: none;
      background: var(--color-white);
      border: 1px solid var(--color-border);
      border-top: none;
      border-radius: 0 0 var(--radius) var(--radius);
      box-shadow: var(--shadow);

      li {
        padding: 8px 12px;
        font-size: 13px;
        cursor: pointer;
        &:hover { background: var(--color-primary-light); }
        &.hint { color: var(--color-text-muted); cursor: default; &:hover { background: none; } }
      }
    }
    input.invalid { border-color: var(--color-danger); }
    .selected {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 12px;
      padding: 8px 12px;
      background: var(--color-primary-light);
      border: 1px solid var(--color-border);
      border-radius: var(--radius);
      font-size: 13px;

      &.invalid { border-color: var(--color-danger); }
    }
    .label { min-width: 0; overflow-wrap: anywhere; }
    .change {
      height: auto;
      padding: 0;
      background: none;
      border: none;
      font-size: 12px;
      font-weight: 500;
      color: var(--color-steel);
      text-decoration: underline;
      white-space: nowrap;
    }
  `]
})
export class AddressPickerComponent implements OnDestroy {
  selected = input<AddressDto | null>(null);
  invalid = input(false);

  selectedChange = output<AddressDto | null>();

  term = signal('');
  results = signal<AddressDto[]>([]);
  open = signal(false);

  readonly label = addressLabel;

  private el = inject(ElementRef);
  private addresses = inject(AddressService);
  private term$ = new Subject<string>();

  private sub = this.term$.pipe(
    debounceTime(250),
    distinctUntilChanged(),
    switchMap(term =>
      this.addresses.search({
        name: term.trim() || undefined,
        pageSize: 8,
        sortBy: 'name',
        sortDirection: 'asc'
      }).pipe(catchError(() => of(null)))
    )
  ).subscribe(r => this.results.set(r?.items ?? []));

  onInput(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.term.set(value);
    this.open.set(true);
    this.term$.next(value);
  }

  onFocus() {
    this.open.set(true);
    this.term$.next(this.term());
  }

  select(address: AddressDto) {
    this.selectedChange.emit(address);
    this.open.set(false);
  }

  clear() {
    this.selectedChange.emit(null);
    this.term.set('');
    this.results.set([]);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.open.set(false);
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
