import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { LoginRequest } from '../auth/login/login-request.type';
import { catchError, throwError, Observable, of, tap, map, shareReplay } from 'rxjs';
import { IsAuthenticated } from '../shared/types/is-authenticated-.type';

export type CurrentUser = {
  userName: string;
  roles: string[];
};

@Service()
export class AuthService {
  http = inject(HttpClient);

  loginUrl = '/api/auth/login';
  logoutUrl = '/api/auth/logout';
  sessionUrl = '/api/auth/session';
  meUrl = '/api/auth/me';

  private user: CurrentUser | null = null;
  private me$?: Observable<CurrentUser | null>;

  login(request: LoginRequest) {
    return this.http.post<void>(this.loginUrl, request).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;

        return throwError(() => new Error(backendMessage || 'Login failed.'));
      }),
    );
  }

  logout() {
    return this.http.delete<void>(this.logoutUrl).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;

        return throwError(() => new Error(backendMessage || 'Logout failed.'));
      }),
    );
  }

  loadMe(): Observable<CurrentUser | null> {
    if (this.me$) return this.me$;

    this.me$ = this.http.get<CurrentUser>(this.meUrl).pipe(
      tap((u) => (this.user = u)),
      map((u) => u ?? null),
      catchError(() => of(null)),
      shareReplay(1),
    );

    return this.me$;
  }

  // Only works if loadMe() has been called - use in Auth & Admin Guards
  isLoggedIn(): boolean {
    return !!this.user;
  }

  // Used to check if there is a session to see if there is a user that is logged in/authenticated
  private sessionResource = httpResource<IsAuthenticated>(() => ({
    url: this.sessionUrl,
  }));
  isSession() {
    return this.sessionResource;
  }
  refreshSession() {
    this.sessionResource.reload();
  }

  hasRole(role: string): boolean {
    return this.user?.roles?.includes(role) ?? false;
  }
}
