import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BookingsByDayResponse } from './bookings-by-day-response.interface';
import { BookingsByDayService } from './bookings-by-day.service';

@Component({
  selector: 'app-bookings-by-day',
  imports: [],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay {
  http = inject(HttpClient);
  bookingsByDayService = inject(BookingsByDayService);
  cd = inject(ChangeDetectorRef);

  isLoading: boolean = false;
  error: string | null = null;
  bookings: BookingsByDayResponse | null = null;

  ngOnInit() {
    this.isLoading = true;
    this.bookingsByDayService.getBookingsByDay().subscribe({
      next: (data) => {
        this.bookings = data;
        this.isLoading = false;
        this.error = null;
        this.cd.detectChanges();
      },
      error: (err) => {
        this.error = err?.error?.message ?? err?.message ?? 'An error occurred';
        this.bookings = null;
        this.isLoading = false;
      },
    });
  }
}
