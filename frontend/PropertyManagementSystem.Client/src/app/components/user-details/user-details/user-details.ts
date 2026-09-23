import { Component, DestroyRef, inject, signal } from '@angular/core';
import { GetUserByIdService } from '../../../services/users/get-user-by-id.service';
import { ActivatedRoute } from '@angular/router';
import { UserDetailsResponse } from '../../../types/users/user-details-response.type';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-user-details',
  imports: [],
  templateUrl: './user-details.html',
  styleUrl: './user-details.css',
})
export class UserDetails {
  getUserByIdService = inject(GetUserByIdService);
  route = inject(ActivatedRoute);
  destroyRef = inject(DestroyRef);
  user = signal<UserDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.isLoading.set(true);
    const userId = this.route.snapshot.params['id'];
    this.getUserByIdService
      .getUser(userId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: UserDetailsResponse) => {
          this.user.set(data);
          this.isLoading.set(false);
          this.errorMessage.set(null);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred.');
          this.user.set(null);
          this.isLoading.set(false);
        },
      });
  }
}
