import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { LoginRequest } from '../auth/login/login-request.interface';
import { catchError, throwError } from 'rxjs';

@Service()
export class AuthService {
  http = inject(HttpClient);
  url = '/api/auth/login';

  login(request: LoginRequest) {
    return this.http.post<void>(this.url, request, { withCredentials: true }).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;

        return throwError(() => new Error(backendMessage || 'Login failed.'));
      }),
    );
  }
}
