import { Component, output } from '@angular/core';

@Component({
  selector: 'app-navbar',
  standalone: true,
  template: `
    <header class="navbar">
      <button
        type="button"
        class="menu-btn"
        aria-label="Navigation ein- oder ausklappen"
        (click)="toggleSidebar.emit()">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none"
             stroke="currentColor" stroke-width="2.2" stroke-linecap="round">
          <path d="M4 7h16M4 12h16M4 17h16" />
        </svg>
      </button>
      <span class="brand">TMS</span>
    </header>
  `,
  styles: [`
    .navbar {
      display: flex;
      align-items: center;
      gap: 12px;
      height: var(--navbar-height);
      padding: 0 20px;
      background: var(--color-steel);
      color: var(--color-white);
    }
    .menu-btn {
      display: grid;
      place-items: center;
      width: 32px;
      height: 32px;
      padding: 0;
      background: var(--color-primary);
      color: var(--color-white);
      border-radius: var(--radius);
    }
    .brand {
      font-size: 18px;
      font-weight: 700;
      letter-spacing: 0.3px;
    }

    @media (max-width: 768px) {
      .navbar { padding: 0 16px; }
    }
  `]
})
export class NavbarComponent {
  toggleSidebar = output<void>();
}