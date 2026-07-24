import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginRequest } from './login-request.interface';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

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
        const msg = err?.error ?? 'Login failed';
        this.errorMessage = msg;
        this.cd.detectChanges();
      },
    });
  }
}
