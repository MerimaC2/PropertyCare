import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { AuthFacadeService } from '../services/auth-facade.service';

/**
 * Guards a route so only authenticated users with one of the roles
 * listed in route data ({ roles: ['Administrator'] }) can enter.
 */
export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authFacade = inject(AuthFacadeService);
  const router = inject(Router);

  if (!authFacade.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  const allowedRoles = (route.data['roles'] as string[] | undefined) ?? [];
  const userRole = authFacade.getCurrentUser()?.role;

  if (allowedRoles.length > 0 && (!userRole || !allowedRoles.includes(userRole))) {
    // Authenticated, but the role is not allowed here.
    return router.createUrlTree([authFacade.homeUrlForCurrentUser()]);
  }

  return true;
};
