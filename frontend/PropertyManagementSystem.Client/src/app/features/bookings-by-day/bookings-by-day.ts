import { ChangeDetectorRef, Component, inject, signal } from '@angular/core';
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
  bookings = signal<BookingsByDayResponse[]>([]);

  ngOnInit() {
    this.isLoading = true;
    this.bookingsByDayService.getBookingsByDay().subscribe({
      next: (data: BookingsByDayResponse[]) => {
        console.log('API response (data):', data, 'isArray:', Array.isArray(data));
        this.bookings.set(data);
        this.isLoading = false;
        this.error = null;
        this.cd.detectChanges();
      },
      error: (err) => {
        this.error = err.message || 'An error occurred';
        this.bookings.set([]);
        this.isLoading = false;
      },
    });

    console.log('Bookings: ', this.bookings);
  }
}
