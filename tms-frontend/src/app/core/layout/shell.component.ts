import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../../shared/components/navbar.component';
import { SidebarComponent } from '../../shared/components/sidebar.component';
import { ToastService } from '../../shared/services/toast.service';
import { AuthService } from '../auth/auth.service';

const MOBILE_BREAKPOINT = 768;

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, SidebarComponent],
  template: `
    <app-navbar (toggleSidebar)="sidebarOpen.update(v => !v)" />
    <div class="layout">
      <app-sidebar
        [open]="sidebarOpen()"
        (logout)="onLogout()"
        (closeRequested)="sidebarOpen.set(false)" />
      <main class="content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .layout {
      display: flex;
      height: calc(100vh - var(--navbar-height));
    }
    .content {
      flex: 1;
      min-width: 0;
      padding: 32px;
      overflow-y: auto;
      background: var(--color-bg);
    }

    @media (max-width: 768px) {
      .content { padding: 16px; }
    }
  `]
})
export class ShellComponent {
  private auth = inject(AuthService);
  private toast = inject(ToastService);

  sidebarOpen = signal(window.innerWidth > MOBILE_BREAKPOINT);

  onLogout() {
    this.auth.logout();
    this.toast.show('Erfolgreich abgemeldet.', 'success');
  }
}