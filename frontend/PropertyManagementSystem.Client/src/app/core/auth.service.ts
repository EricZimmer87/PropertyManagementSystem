import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { LoginRequest } from '../auth/login/login-request.interface';
import { catchError, throwError, Observable, of, tap, map, shareReplay } from 'rxjs';

export type CurrentUser = {
  userName: string;
  roles: string[];
};

@Service()
export class AuthService {
  http = inject(HttpClient);

  loginUrl = '/api/auth/login';

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

  loadMe(): Observable<CurrentUser | null> {
    if (this.me$) return this.me$;

    this.me$ = this.http.get<CurrentUser>('/api/auth/me').pipe(
      tap((u) => (this.user = u)),
      map((u) => u ?? null),
      catchError(() => of(null)),
      shareReplay(1),
    );

    return this.me$;
  }

  isLoggedIn(): boolean {
    return !!this.user;
  }

  hasRole(role: string): boolean {
    return this.user?.roles?.includes(role) ?? false;
  }
}
