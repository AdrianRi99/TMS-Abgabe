import {
  Component, input, output, signal,
  HostListener, ElementRef, inject, OnDestroy
} from '@angular/core';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-autocomplete',
  standalone: true,
  template: `
    <div class="autocomplete-wrapper">
      <input
        [value]="value()"
        [placeholder]="placeholder()"
        [class.invalid]="invalid()"
        (input)="onInput($event)"
        (focus)="onFocus()"
        autocomplete="off" />
      @if (showDropdown() && options().length > 0) {
        <ul class="dropdown" role="listbox">
          @for (option of options(); track option) {
            <li role="option" (mousedown)="select(option)">{{ option }}</li>
          }
        </ul>
      }
    </div>
  `,
  styles: [`
    .autocomplete-wrapper { position: relative; }
    .dropdown {
      position: absolute;
      top: 100%;
      left: 0;
      right: 0;
      background: var(--color-white);
      border: 1px solid var(--color-border);
      border-top: none;
      border-radius: 0 0 var(--radius) var(--radius);
      list-style: none;
      max-height: 200px;
      overflow-y: auto;
      z-index: 50;
      box-shadow: var(--shadow);

      li {
        padding: 8px 12px;
        cursor: pointer;
        font-size: 13px;
        &:hover { background: var(--color-primary-light); }
      }
    }
    input.invalid { border-color: var(--color-danger); }
  `]
})
export class AutocompleteComponent implements OnDestroy {
  value = input('');
  suggestions = input<string[]>([]);
  placeholder = input('');
  invalid = input(false);

  valueChange = output<string>();

  showDropdown = signal(false);
  options = signal<string[]>([]);

  private el = inject(ElementRef);
  private term$ = new Subject<string>();

  private sub = this.term$.pipe(
    debounceTime(150),
    distinctUntilChanged(),
    switchMap(term => {
      const all = this.suggestions();
      if (!term.trim()) return of(all);
      const lower = term.toLowerCase();
      return of(all.filter(s => s.toLowerCase().includes(lower)));
    })
  ).subscribe(results => this.options.set(results));

  onInput(event: Event) {
    const val = (event.target as HTMLInputElement).value;
    this.valueChange.emit(val);
    this.showDropdown.set(true);
    this.term$.next(val);
  }

  onFocus() {
    this.showDropdown.set(true);
    this.term$.next(this.value());
  }

  select(option: string) {
    this.valueChange.emit(option);
    this.options.set([]);
    this.showDropdown.set(false);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.showDropdown.set(false);
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}