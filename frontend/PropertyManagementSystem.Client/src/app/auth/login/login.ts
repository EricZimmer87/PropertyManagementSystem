import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginRequest } from './login-request.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { toFriendlyError } from '../../shared/error-messages.ts/to-friendly-error';

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
  private activatedRoute = inject(ActivatedRoute);

  errorMessage: string | null = null;

  ngOnInit() {
    // If Google OAuth login fails, it redirects with a query parameter for the error
    // Use this to display that error message on the login HTML page
    this.activatedRoute.queryParams.subscribe((params) => {
      this.errorMessage = params['error'] ? toFriendlyError(params['error']) : '';
    });
  }

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

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
    // Set where to return to after Google login
    const returnUrl = this.router.url || '/bookings-by-day';
    const url = `https://localhost:7016/api/auth/signin-google?returnUrl=${encodeURIComponent(returnUrl)}`;

    window.location.href = url;
  }
}
