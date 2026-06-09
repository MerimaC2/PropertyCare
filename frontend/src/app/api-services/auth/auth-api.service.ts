import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginCommand, LoginCommandDto, LogoutCommand, RefreshTokenCommand } from './auth-api.model';

/** Thin HTTP client for the backend /api/auth endpoints. */
@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient) {}

  login(command: LoginCommand): Observable<LoginCommandDto> {
    return this.http.post<LoginCommandDto>(`${this.apiUrl}/login`, command);
  }

  refresh(command: RefreshTokenCommand): Observable<LoginCommandDto> {
    return this.http.post<LoginCommandDto>(`${this.apiUrl}/refresh`, command);
  }

  logout(command: LogoutCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, command);
  }
}
