import { Component, DestroyRef, inject, signal } from '@angular/core';
import { GetGuestByIdService } from '../../services/guests/get-guest-by-id.service';
import { ActivatedRoute } from '@angular/router';
import { GuestDetailsResponse } from '../../types/guests/guest-details-response.type';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-guest-details.ts',
  imports: [],
  templateUrl: './guest-details.html',
  styleUrl: './guest-details.css',
})
export class GuestDetails {
  getGuestByIdService = inject(GetGuestByIdService);
  route = inject(ActivatedRoute);
  destroyRef = inject(DestroyRef);
  guest = signal<GuestDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.isLoading.set(true);
    const guestId = parseInt(this.route.snapshot.params['id']);
    this.getGuestByIdService
      .getGuest(guestId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: GuestDetailsResponse) => {
          this.guest.set(data);
          this.isLoading.set(false);
          this.errorMessage.set(null)
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred.');
          this.guest.set(null);
          this.isLoading.set(false);
        },
      });
  }
}
