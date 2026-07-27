import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginRequest } from './login-request.interface';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { HttpClient } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private router = inject(Router);
  private cd = inject(ChangeDetectorRef);
  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private document = inject(DOCUMENT);

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  errorMessage: string | null = null;

  submitLogin() {
    this.errorMessage = null;
    const email = this.loginForm.value.email ?? '';
    const password = this.loginForm.value.password ?? '';
    const request: LoginRequest = { email, password };

    this.authService.login(request).subscribe({
      next: () => {
        this.router.navigate(['/bookings-by-day']);
      },
      error: (err) => {
        this.errorMessage = err.message || 'Login failed.';
        this.cd.detectChanges();
      },
    });
  }

  googleLoginSubmit() {
    console.log('Google login clicked.');
    var url = 'http://localhost:5093/api/auth/signin-google';

    this.http.get<{ authURL: string }>(url).subscribe({
      next: (data) => {
        // Redirect the entire browser to the Google OAuth URL
        this.document.location.href = data.authURL;
      },
      error: (err) => console.error('OAuth redirect failed', err),
    });
  }
}
