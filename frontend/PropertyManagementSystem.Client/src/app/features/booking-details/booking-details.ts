import { Component, DestroyRef, inject, signal } from '@angular/core';
import { GetBookingByIdService } from '../../shared/booking-by-id/get-booking-by-id.service';
import { ActivatedRoute } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BookingDetailsResponse } from '../../shared/types/booking-details-response.type';
import { DatePipe } from '@angular/common';
import { BookingStatusLabelPipe } from '../../shared/pipes/booking-status-label.pipe';

@Component({
  selector: 'app-booking-details',
  imports: [DatePipe, BookingStatusLabelPipe],
  templateUrl: './booking-details.html',
  styleUrl: './booking-details.css',
})
export class BookingDetails {
  getBookingByIdService = inject(GetBookingByIdService);
  route = inject(ActivatedRoute);
  destroyRef = inject(DestroyRef);
  booking = signal<BookingDetailsResponse | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  constructor() {
    this.isLoading.set(true);
    const bookingId = parseInt(this.route.snapshot.params['id']);
    this.getBookingByIdService
      .getBooking(bookingId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: BookingDetailsResponse) => {
          this.booking.set(data);
          this.isLoading.set(false);
          this.errorMessage.set(null);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred.');
          this.booking.set(null);
          this.isLoading.set(false);
        },
      });
  }
}
