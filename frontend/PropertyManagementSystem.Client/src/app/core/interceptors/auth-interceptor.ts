import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

let navigationInProgress = false;

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<any>> => {
  const router = inject(Router);

  // Skip redirects for login
  const isAuthRequest =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/resend-confirmation-email') ||
    req.url.includes('/auth/confirm-email') ||
    req.url.includes('/auth/forgot-password') ||
    req.url.includes('/auth/reset-password') ||
    req.url.includes('/auth/signin-google') ||
    req.url.includes('/auth/google-signin-callback');

  return next(req).pipe(
    tap({
      error: (error: unknown) => {
        if (isAuthRequest) return;
        if (navigationInProgress) return;

        if (error instanceof HttpErrorResponse) {
          if (error.status === 401) {
            navigationInProgress = true;
            router.navigate(['login']).finally(() => {
              navigationInProgress = false;
            });
          } else if (error.status === 403) {
            navigationInProgress = true;
            router.navigate(['unauthorized']).finally(() => {
              navigationInProgress = false;
            });
          }
        }
      },
    }),
  );
};
