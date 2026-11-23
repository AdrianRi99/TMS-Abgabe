import { Component, input, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

interface NavItem {
  label: string;
  path: string;
  /** SVG path data (one entry per <path>), 24x24 viewBox, stroke style. */
  icon: string[];
  /** Active only on an exact URL match (needed for the root path). */
  exact?: boolean;
}

const NAV_ITEMS: NavItem[] = [
  {
    label: 'Adressverwaltung',
    path: '/',
    exact: true,
    icon: ['M3 11.5 12 4l9 7.5', 'M5 10v10h14V10']
  },
  {
    label: 'Bestellungen',
    path: '/orders',
    icon: [
      'M9 4h6a1 1 0 0 1 1 1v1H8V5a1 1 0 0 1 1-1z',
      'M8 6H6a1 1 0 0 0-1 1v13a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V7a1 1 0 0 0-1-1h-2',
      'M9 12h6M9 16h4'
    ]
  }
];

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    @if (open()) {
      <div class="backdrop" (click)="closeRequested.emit()"></div>
      <aside class="sidebar">
        <div>
          <p class="nav-label">NAVIGATION</p>
          @for (item of navItems; track item.path) {
            <a class="nav-item" [routerLink]="item.path" routerLinkActive="active"
               [routerLinkActiveOptions]="{ exact: item.exact ?? false }">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                   stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                @for (d of item.icon; track d) {
                  <path [attr.d]="d" />
                }
              </svg>
              {{ item.label }}
            </a>
          }
        </div>
        <div class="sidebar-bottom">
          <button type="button" class="logout-btn" (click)="logout.emit()">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                 stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
              <path d="M16 17l5-5-5-5" />
              <path d="M21 12H9" />
            </svg>
            Abmelden
          </button>
        </div>
      </aside>
    }
  `,
  styles: [`
    :host { display: flex; }

    .sidebar {
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      width: 220px;
      flex-shrink: 0;
      padding: 20px 12px 12px;
      background: var(--color-white);
      border-right: 1px solid var(--color-border);
    }
    .nav-label {
      padding: 0 12px 10px;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.08em;
      color: #8a9bb0;
    }
    .nav-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 10px 12px;
      border-radius: var(--radius);
      font-size: 13px;
      font-weight: 500;
      color: var(--color-text);
      text-decoration: none;
      cursor: pointer;

      &.active {
        background: var(--color-primary-light);
        color: var(--color-steel);
        font-weight: 600;
      }
    }
    .sidebar-bottom {
      padding-top: 12px;
      border-top: 1px solid var(--color-border-light);
    }
    .logout-btn {
      display: flex;
      align-items: center;
      gap: 10px;
      width: 100%;
      height: auto;
      padding: 10px 12px;
      background: transparent;
      border: none;
      font-size: 13px;
      font-weight: 500;
      color: var(--color-text-muted);
      text-align: left;

      &:hover { background: var(--color-bg); color: var(--color-text); }
    }
    .backdrop { display: none; }

    @media (max-width: 768px) {
      .sidebar {
        position: fixed;
        top: var(--navbar-height);
        bottom: 0;
        left: 0;
        z-index: 60;
        box-shadow: 4px 0 16px rgba(17, 29, 44, 0.15);
      }
      .backdrop {
        display: block;
        position: fixed;
        top: var(--navbar-height);
        right: 0;
        bottom: 0;
        left: 0;
        z-index: 55;
        background: rgba(17, 29, 44, 0.35);
      }
    }
  `]
})
export class SidebarComponent {
  readonly navItems = NAV_ITEMS;

  open = input(true);
  logout = output<void>();
  closeRequested = output<void>();
}