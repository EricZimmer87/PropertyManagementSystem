import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) return of(true);

  return auth.loadMe().pipe(
    map((user) => {
      if (user) return true;
      router.navigate(['/'], { queryParams: { error: 'not_logged_in' } });
      return false;
    }),
    catchError(() => {
      router.navigate(['/'], { queryParams: { error: 'auth_failed' } });
      return of(false);
    }),
  );
};
