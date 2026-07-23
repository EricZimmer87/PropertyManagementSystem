import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginService } from './login.service';
import { LoginRequest } from './login-request.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private router = inject(Router);
  loginService = inject(LoginService);

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  errorMessage: string | null = null;

  async submitLogin() {
    this.errorMessage = null;

    const email = this.loginForm.value.email ?? '';
    const password = this.loginForm.value.password ?? '';
    const request: LoginRequest = { email, password };

    try {
      await this.loginService.submitLogin(request);
      this.router.navigate(['/bookings-by-day']);
    } catch (err: any) {
      const msg = err?.error ?? 'Login failed';
      this.errorMessage = msg;
    }
  }
}
