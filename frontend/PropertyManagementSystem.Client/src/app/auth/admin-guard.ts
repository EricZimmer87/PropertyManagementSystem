import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';
import { inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';
import { Roles } from '../enums/roles.enum';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // If already loaded, decide immediately
  if (auth.isLoggedIn()) {
    return auth.hasRole(Roles.Admin)
      ? of(true)
      : of(router.createUrlTree(['/forbidden'], { queryParams: { error: 'forbidden' } }));
  }

  return auth.loadMe().pipe(
    map((user) => {
      if (user?.roles.includes(Roles.Admin)) {
        return true;
      }
      return router.createUrlTree(['/forbidden'], { queryParams: { error: 'forbidden' } });
    }),
    catchError(() => of(router.createUrlTree(['/'], { queryParams: { error: 'auth_failed' } }))),
  );
};
