import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) return of(true);

  return auth.loadMe().pipe(
    map((user) =>
      user ? true : router.createUrlTree(['/'], { queryParams: { error: 'not_logged_in' } }),
    ),
    catchError(() => of(router.createUrlTree(['/'], { queryParams: { error: 'auth_failed' } }))),
  );
};
