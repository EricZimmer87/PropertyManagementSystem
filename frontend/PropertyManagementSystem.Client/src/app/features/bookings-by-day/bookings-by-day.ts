import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BookingByDay } from './booking-by-day.type';
import { BookingsByDayService } from './bookings-by-day.service';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-bookings-by-day',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay implements OnInit {
  bookingsByDayService = inject(BookingsByDayService);
  cd = inject(ChangeDetectorRef);
  destroyRef = inject(DestroyRef);

  isLoading: boolean = true;
  error: string | null = null;
  bookings: BookingByDay[] = [];

  selectDayForm = new FormGroup({
    selectedDay: new FormControl(''),
  });

  ngOnInit() {
    this.getBookingsByDay();
  }

  getBookingsByDay() {
    this.isLoading = true;
    const selectedDay = this.selectDayForm.value.selectedDay;
    const params = selectedDay ? new HttpParams().set('selectedDay', selectedDay) : undefined;

    this.bookingsByDayService
      .getBookingsByDay(params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
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
