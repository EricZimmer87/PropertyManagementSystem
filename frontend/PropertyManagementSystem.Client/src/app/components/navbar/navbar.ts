import { Component, computed, inject, signal } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  errorMessage = signal<string | null>(null);

  readonly isLoggedInResource = this.authService.isSession();

  readonly isLoggedIn = computed(() =>
    this.isLoggedInResource.hasValue() ? this.isLoggedInResource.value().isAuthenticated : null,
  );

  logout() {
    this.errorMessage.set(null);

    this.authService
      .logout()
      .pipe()
      .subscribe({
        next: () => {
          this.authService.refreshSession();
          this.router.navigate(['/']);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'Logout failed.');
        },
      });
  }
}
