import { Component, DestroyRef, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginRequest } from './login-request.type';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { toFriendlyError } from '../../shared/error-messages.ts/to-friendly-error';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);
  private authService = inject(AuthService);
  private activatedRoute = inject(ActivatedRoute);

  errorMessage = signal<string | null>(null);
  isSubmitting = signal(false);

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  constructor() {
    // If Google OAuth login fails, it redirects with a query parameter for the error.
    // Use this to display that error message on the login HTML page.
    this.errorMessage.set(
      toFriendlyError(this.activatedRoute.snapshot.queryParams['error'] ?? null),
    );
  }

  submitLogin() {
    if (this.isSubmitting()) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    const email = this.loginForm.value.email ?? '';
    const password = this.loginForm.value.password ?? '';
    const request: LoginRequest = { email, password };

    this.authService
      .login(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.authService.refreshSession();
          this.router.navigate(['/bookings-by-day']);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'Login failed.');
          this.isSubmitting.set(false);
        },
      });
  }

  googleLoginSubmit() {
    const returnUrl = this.router.url || '/bookings-by-day';
    const url = `https://localhost:7016/api/auth/signin-google?returnUrl=${encodeURIComponent(returnUrl)}`;
    window.location.href = url;
  }
}
