import {
  HttpErrorResponse,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthApiService } from '../../api-services/auth/auth-api.service';
import { AuthFacadeService } from '../services/auth-facade.service';

let refreshInProgress = false;
const refreshedToken$ = new BehaviorSubject<string | null>(null);

/**
 * The access token belongs to this application's API and to nothing else, so it is never
 * attached to a request aimed at a third-party host.
 */
const isAppApi = (url: string): boolean => url.startsWith(environment.apiUrl);

const isAuthEndpoint = (url: string): boolean =>
  url.includes('/api/auth/login') ||
  url.includes('/api/auth/refresh') ||
  url.includes('/api/auth/logout');

const withBearer = (request: HttpRequest<unknown>, token: string): HttpRequest<unknown> =>
  request.clone({ setHeaders: { Authorization: `Bearer ${token}` } });

/**
 * Adds the JWT access token to requests aimed at the application API and transparently
 * refreshes it on 401 responses. Concurrent requests wait for the single refresh call to
 * finish and are then retried. Requests to any other host are passed through untouched.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authFacade = inject(AuthFacadeService);
  const authApi = inject(AuthApiService);
  const router = inject(Router);

  const token = authFacade.getAccessToken();
  if (token && isAppApi(request.url) && !isAuthEndpoint(request.url)) {
    request = withBearer(request, token);
  }

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      // Only a 401 from our own API means the session expired; a foreign host answering 401
      // is not our session, and refreshing on it would send the token where it does not belong.
      if (error.status === 401 && isAppApi(request.url) && !isAuthEndpoint(request.url)) {
        return handle401(request, next, authFacade, authApi, router);
      }
      return throwError(() => error);
    })
  );
};

function handle401(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
  authFacade: AuthFacadeService,
  authApi: AuthApiService,
  router: Router
) {
  const refreshToken = authFacade.getRefreshToken();
  if (!refreshToken) {
    authFacade.clearSession();
    router.navigate(['/login']);
    return throwError(() => new Error('Not authenticated.'));
  }

  if (!refreshInProgress) {
    refreshInProgress = true;
    refreshedToken$.next(null);

    return authApi.refresh({ refreshToken }).pipe(
      switchMap(result => {
        refreshInProgress = false;
        authFacade.storeSession(result);
        refreshedToken$.next(result.accessToken);
        return next(withBearer(request, result.accessToken));
      }),
      catchError(refreshError => {
        refreshInProgress = false;
        authFacade.clearSession();
        router.navigate(['/login']);
        return throwError(() => refreshError);
      })
    );
  }

  // Another request already triggered the refresh - wait for the new token.
  return refreshedToken$.pipe(
    filter((newToken): newToken is string => newToken !== null),
    take(1),
    switchMap(newToken => next(withBearer(request, newToken)))
  );
}
