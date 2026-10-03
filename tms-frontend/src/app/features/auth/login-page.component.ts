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
  template: `
    <div class="page">
      <div class="card">
        <h1>TMS</h1>

        <form novalidate (submit)="onSubmit($event)">
          <div class="field">
            <label for="email">E-Mail</label>
            <input
              id="email"
              type="email"
              autocomplete="email"
              placeholder="name@firma.de"
              [value]="email()"
              [class.invalid]="submitted() && !emailValid()"
              (input)="email.set(getValue($event))" />
            @if (submitted() && !emailValid()) {
              <span class="error-msg">Bitte eine gültige E-Mail-Adresse eingeben</span>
            }
          </div>

          <div class="field">
            <label for="password">Passwort</label>
            <input
              id="password"
              type="password"
              [attr.autocomplete]="isLogin() ? 'current-password' : 'new-password'"
              [placeholder]="isLogin() ? '••••••••' : 'Mindestens 8 Zeichen'"
              [value]="password()"
              [class.invalid]="submitted() && !passwordValid()"
              (input)="password.set(getValue($event))" />
            @if (submitted() && !passwordValid()) {
              <span class="error-msg">
                {{ isLogin() ? 'Pflichtfeld' : 'Mindestens 8 Zeichen' }}
              </span>
            }
          </div>

          @if (!isLogin()) {
            <div class="field">
              <label for="confirm">Passwort bestätigen</label>
              <input
                id="confirm"
                type="password"
                autocomplete="new-password"
                placeholder="••••••••"
                [value]="confirm()"
                [class.invalid]="submitted() && !confirmValid()"
                (input)="confirm.set(getValue($event))" />
              @if (submitted() && !confirmValid()) {
                <span class="error-msg">Passwörter stimmen nicht überein</span>
              }
            </div>
          }

          @if (serverError()) {
            <p class="server-error" role="alert">{{ serverError() }}</p>
          }

          <button type="submit" class="btn-primary submit" [disabled]="loading()">
            {{ isLogin() ? 'Anmelden' : 'Registrieren' }}
          </button>
        </form>

        <div class="divider">
          <span>oder</span>
        </div>

        <button
          type="button"
          class="btn-demo"
          [disabled]="loading()"
          (click)="loginAsDemo()">
          Demo-Zugang nutzen
        </button>

        <p class="switch">
          {{ isLogin() ? 'Noch kein Konto?' : 'Bereits registriert?' }}
          <button type="button" class="link" (click)="toggleMode()">
            {{ isLogin() ? 'Registrieren' : 'Anmelden' }}
          </button>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .page {
      display: grid;
      place-items: center;
      min-height: 100vh;
      padding: 16px;
      background: var(--color-bg);
    }
    .card {
      width: 400px;
      max-width: 100%;
      padding: 36px 32px 28px;
      background: var(--color-white);
      border: 1px solid var(--color-border-light);
      border-radius: 12px;
      box-shadow: 0 2px 12px rgba(17, 29, 44, 0.08);
    }
    h1 {
      margin-bottom: 28px;
      text-align: center;
      font-size: 28px;
      font-weight: 700;
      color: var(--color-primary);
    }
    .field {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-bottom: 16px;

      label { font-size: 13px; font-weight: 600; }
    }
    .error-msg { font-size: 12px; color: var(--color-danger); }
    .server-error {
      margin-bottom: 12px;
      padding: 10px 12px;
      font-size: 13px;
      color: var(--color-danger);
      background: #fdf2f2;
      border: 1px solid #f5c6c6;
      border-radius: var(--radius);
    }
    .submit {
      width: 100%;
      height: 40px;
      margin-top: 8px;
    }
    .divider {
      display: flex;
      align-items: center;
      gap: 12px;
      margin: 20px 0 16px;
      color: var(--color-text-muted);
      font-size: 12px;

      &::before, &::after {
        content: '';
        flex: 1;
        height: 1px;
        background: var(--color-border);
      }
    }
    .btn-demo {
      width: 100%;
      height: 40px;
      background: var(--color-white);
      color: var(--color-steel);
      border: 1px solid var(--color-border);
      font-size: 13px;
      font-weight: 600;
      border-radius: var(--radius);
      cursor: pointer;
      transition: background 0.15s;

      &:hover:not(:disabled) { background: var(--color-bg); }
      &:disabled { opacity: 0.5; cursor: not-allowed; }
    }
    .switch {
      margin-top: 16px;
      text-align: center;
      font-size: 12px;
      color: var(--color-text-muted);
    }
    .link {
      height: auto;
      padding: 0;
      background: none;
      border: none;
      font-size: 12px;
      font-weight: 600;
      color: var(--color-steel);
      text-decoration: underline;
      cursor: pointer;
    }
  `]
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
    this.isLogin()
      ? this.password().length > 0
      : this.password().length >= MIN_PASSWORD_LENGTH
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
    this.auth
      .login({ email: this.DEMO_EMAIL, password: this.DEMO_PASSWORD })
      .subscribe({
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