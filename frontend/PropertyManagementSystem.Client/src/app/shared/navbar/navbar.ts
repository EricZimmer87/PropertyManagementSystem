import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class Navbar {
  private readonly authService = inject(AuthService);

  readonly isLoggedInResource = this.authService.isSession();

  readonly isLoggedIn = computed(() =>
    this.isLoggedInResource.hasValue() ? this.isLoggedInResource.value().isAuthenticated : null,
  );
}
