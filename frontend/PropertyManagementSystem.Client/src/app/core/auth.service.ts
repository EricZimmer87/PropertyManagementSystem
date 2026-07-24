import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginRequest } from '../auth/login/login-request.interface';

@Service()
export class AuthService {
  http = inject(HttpClient);
  url = '/api/auth/login';

  login(request: LoginRequest) {
    return this.http.post<void>(this.url, request, { withCredentials: true });
  }
}
