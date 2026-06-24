import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { AuthApiService } from '../../api-services/auth/auth-api.service';
import { APP_ROLES, LoginCommand, LoginCommandDto } from '../../api-services/auth/auth-api.model';

const ACCESS_TOKEN_KEY = 'pc_access_token';
const REFRESH_TOKEN_KEY = 'pc_refresh_token';
const USER_KEY = 'pc_user';

export interface CurrentUser {
  userId: number;
  email: string;
  fullName: string;
  role: string;
}

/**
 * Single place that owns the auth state: token storage,
 * login/logout flows and role checks.
 */
@Injectable({ providedIn: 'root' })
export class AuthFacadeService {
  constructor(
    private authApi: AuthApiService,
    private router: Router
  ) {}

  login(command: LoginCommand): Observable<LoginCommandDto> {
    return this.authApi.login(command).pipe(tap(result => this.storeSession(result)));
  }

  logout(): void {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      // Best-effort revoke; local session is cleared regardless of the result.
      this.authApi.logout({ refreshToken }).subscribe({ error: () => {} });
    }
    this.clearSession();
    this.router.navigate(['/login']);
  }

  storeSession(result: LoginCommandDto): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, result.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, result.refreshToken);
    const user: CurrentUser = {
      userId: result.userId,
      email: result.email,
      fullName: result.fullName,
      role: result.role
    };
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  getCurrentUser(): CurrentUser | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as CurrentUser) : null;
  }

  isLoggedIn(): boolean {
    return !!this.getAccessToken();
  }

  hasRole(role: string): boolean {
    return this.getCurrentUser()?.role === role;
  }

  /** Route to the home screen of the user's role after login. */
  homeUrlForCurrentUser(): string {
    const role = this.getCurrentUser()?.role;
    switch (role) {
      case APP_ROLES.administrator:
        return '/admin/triage';
      case APP_ROLES.technician:
        return '/technician/interventions';
      case APP_ROLES.reporter:
        return '/reporter/my-requests';
      default:
        return '/';
    }
  }
}
