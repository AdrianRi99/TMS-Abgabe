import { Component, computed, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../shared/services/toast.service';

type Mode = 'login' | 'register';

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const MIN_PASSWORD_LENGTH = 8;

@Component({
  selector: 'app-login-page',
  standalone: true,
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  private toast = inject(ToastService);

  private readonly DEMO_EMAIL = 'demo@tms.dev';
  private readonly DEMO_PASSWORD = 'Demo1234';

  mode = signal<Mode>('login');
  email = signal('');
  password = signal('');
  confirm = signal('');
  submitted = signal(false);
  loading = signal(false);
  serverError = signal<string | null>(null);

  isLogin = computed(() => this.mode() === 'login');
  emailValid = computed(() => EMAIL_PATTERN.test(this.email().trim()));
  passwordValid = computed(() =>
    this.isLogin() ? this.password().length > 0 : this.password().length >= MIN_PASSWORD_LENGTH
  );
  confirmValid = computed(() => this.isLogin() || this.confirm() === this.password());
  formValid = computed(() => this.emailValid() && this.passwordValid() && this.confirmValid());

  toggleMode() {
    this.mode.update(m => (m === 'login' ? 'register' : 'login'));
    this.submitted.set(false);
    this.serverError.set(null);
    this.confirm.set('');
  }

  onSubmit(event: Event) {
    event.preventDefault();
    this.submitted.set(true);
    this.serverError.set(null);
    if (!this.formValid()) return;

    const credentials = { email: this.email().trim(), password: this.password() };
    const request = this.isLogin()
      ? this.auth.login(credentials)
      : this.auth.register(credentials);

    this.loading.set(true);
    request.subscribe({
      next: () => {
        if (!this.isLogin()) this.toast.show('Konto angelegt. Willkommen!', 'success');
        this.router.navigate(['/']);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        this.serverError.set(this.toMessage(err));
      }
    });
  }

  loginAsDemo() {
    this.serverError.set(null);
    this.loading.set(true);
    this.auth.login({ email: this.DEMO_EMAIL, password: this.DEMO_PASSWORD }).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.loading.set(false);
        this.serverError.set('Demo-Zugang nicht verfügbar. Ist der Server gestartet?');
      }
    });
  }

  getValue(event: Event): string {
    return (event.target as HTMLInputElement).value;
  }

  private toMessage(err: HttpErrorResponse): string {
    if (err.status === 0) return 'Server nicht erreichbar.';
    if (err.status === 401) return err.error?.detail ?? 'E-Mail oder Passwort ist falsch.';

    const errors = err.error?.errors as Record<string, string[]> | undefined;
    if (errors) return Object.values(errors).flat().join(' ');

    return 'Ein unerwarteter Fehler ist aufgetreten.';
  }
}