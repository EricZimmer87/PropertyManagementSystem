import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { LoginRequest } from './login-request.interface';
import { firstValueFrom } from 'rxjs';

@Service()
export class LoginService {
  private http = inject(HttpClient);

  url = 'http://localhost:5093/api/auth/login';

  async submitLogin(request: LoginRequest): Promise<void> {
    await firstValueFrom(this.http.post<void>(this.url, request, { withCredentials: true }));
  }
}
