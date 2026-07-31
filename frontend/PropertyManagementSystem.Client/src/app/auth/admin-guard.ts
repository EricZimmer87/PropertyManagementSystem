import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../core/auth.service';
import { inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';
import { Roles } from '../shared/enums/roles.enum';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // If already loaded, decide immediately
  if (auth.isLoggedIn()) {
    if (auth.hasRole(Roles.Admin)) return of(true);
    router.navigate(['/forbidden'], { queryParams: { error: 'forbidden' } });
    return of(false);
  }

  // Otherwise load /api/auth/me and decide
  return auth.loadMe().pipe(
    map((user) => {
      const ok = !!user && user.roles.includes(Roles.Admin);
      if (!ok)
        router.navigate(['/forbidden'], { queryParams: { error: 'forbidden' } });
      return ok;
    }),
    catchError(() => {
      router.navigate(['/login'], { queryParams: { error: 'auth_failed' } });
      return of(false);
    }),
  );
};
