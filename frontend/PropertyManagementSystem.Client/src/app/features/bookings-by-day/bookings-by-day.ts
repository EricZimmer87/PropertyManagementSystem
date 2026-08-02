import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BookingByDay } from './booking-by-day.type';
import { BookingsByDayService } from './bookings-by-day.service';

@Component({
  selector: 'app-bookings-by-day',
  imports: [],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay implements OnInit {
  http = inject(HttpClient);
  bookingsByDayService = inject(BookingsByDayService);
  cd = inject(ChangeDetectorRef);

  isLoading: boolean = true;
  error: string | null = null;
  bookings: BookingByDay[] = [];

  ngOnInit() {
    this.isLoading = true;

    this.bookingsByDayService.getBookingsByDay().subscribe({
      next: (data: BookingByDay[]) => {
        this.bookings = data;
        this.isLoading = false;
        this.error = null;
        this.cd.detectChanges();
      },
      error: (err) => {
        this.error = err.message || 'An error occurred';
        this.bookings = [];
        this.isLoading = false;
        this.cd.detectChanges();
      },
    });
  }
}
