import { Service, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { LoginRequest } from '../../auth/login/login-request.type';
import { catchError, throwError, Observable, of, tap, map, shareReplay } from 'rxjs';
import { IsAuthenticated } from '../../types/auth/is-authenticated-.type';
import { Roles } from '../../enums/roles.enum';

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

  private currentUserSignal = signal<CurrentUser | null>(null);
  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAdmin = computed(() => this.currentUserSignal()?.roles.includes(Roles.Admin) ?? false);

  // This is called when the app initializes to avoid signals being reset when user refreshes the page
  loadCurrentUser(): Observable<CurrentUser | null> {
    return this.http.get<CurrentUser>(this.meUrl).pipe(
      tap((user) => {
        this.currentUserSignal.set(user);
      }),
      catchError(() => {
        this.currentUserSignal.set(null);
        return of(null);
      }),
      shareReplay(1)
    );
  }

  login(request: LoginRequest): Observable<CurrentUser | null> {
    return this.http.post<CurrentUser>(this.loginUrl, request).pipe(
      tap((user) => {
        this.currentUserSignal.set(user);
      }),
      catchError(() => {
        this.currentUserSignal.set(null);
        return of(null);
      }),
    );
  }

  logout(): Observable<void> {
    return this.http.delete<void>(this.logoutUrl).pipe(
      tap(() => {
        this.currentUserSignal.set(null);
      }),
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;

        return throwError(() => new Error(backendMessage || 'Logout failed.'));
      }),
    );
  }
}
