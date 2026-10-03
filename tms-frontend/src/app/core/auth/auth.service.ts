import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, map, tap } from 'rxjs';
import { API_URL } from '../api-config';

export interface Credentials {
  email: string;
  password: string;
}

interface AuthSession {
  token: string;
  expiresAtUtc: string;
  email: string;
}

const STORAGE_KEY = 'tms.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private session = signal<AuthSession | null>(this.restore());

  readonly token = computed(() => this.session()?.token ?? null);

  readonly isAuthenticated = computed(() => {
    const s = this.session();
    return !!s && new Date(s.expiresAtUtc) > new Date();
  });

  login(credentials: Credentials): Observable<void> {
    return this.http
      .post<AuthSession>(`${API_URL}/auth/login`, credentials)
      .pipe(tap(s => this.setSession(s)), map(() => undefined));
  }

  register(credentials: Credentials): Observable<void> {
    return this.http
      .post<AuthSession>(`${API_URL}/auth/register`, credentials)
      .pipe(tap(s => this.setSession(s)), map(() => undefined));
  }

  logout() {
    localStorage.removeItem(STORAGE_KEY);
    this.session.set(null);
    this.router.navigate(['/login']);
  }

  private setSession(session: AuthSession) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    this.session.set(session);
  }

  private restore(): AuthSession | null {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      return raw ? (JSON.parse(raw) as AuthSession) : null;
    } catch {
      return null;
    }
  }
}