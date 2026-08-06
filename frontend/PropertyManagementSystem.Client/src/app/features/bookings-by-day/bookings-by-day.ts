import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BookingByDay } from './booking-by-day.type';
import { BookingsByDayService } from './bookings-by-day.service';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BookingsByDayResponse } from './bookings-by-day-response.type';
import { RouterLink } from '@angular/router';
import { BookingStatusLabelPipe } from '../../shared/pipes/booking-status-label.pipe';
import { BookingStatus } from '../../shared/enums/booking-status.enum';

@Component({
  selector: 'app-bookings-by-day',
  imports: [DatePipe, ReactiveFormsModule, RouterLink, BookingStatusLabelPipe],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay {
  public readonly BookingStatus = BookingStatus;
  private readonly bookingsByDayService = inject(BookingsByDayService);
  destroyRef = inject(DestroyRef);

  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  bookings = signal<BookingByDay[]>([]);
  selectedDay = signal<string | null>(null);

  doubleBookedUnits = computed(() => {
    return this.bookingsByDayService.findDoubleBookedUnits(this.bookings());
  });

  selectDayForm = new FormGroup({
    selectedDay: new FormControl(''),
  });

  constructor() {
    this.getBookingsByDay();
  }

  getBookingsByDay(selectedDay?: string) {
    this.isLoading.set(true);
    const params = selectedDay ? new HttpParams().set('selectedDay', selectedDay) : undefined;

    this.bookingsByDayService
      .getBookingsByDay(params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data: BookingsByDayResponse) => {
          this.bookings.set(data.bookings);
          this.selectedDay.set(data.selectedDay);
          this.errorMessage.set(null);
          this.isLoading.set(false);
        },
        error: (err) => {
          this.errorMessage.set(err.message || 'An error occurred');
          this.bookings.set([]);
          this.isLoading.set(false);
        },
      });
  }

  nextDayBookingsByDay() {
    const selectedDay = this.selectedDay();
    if (!selectedDay) {
      return;
    }

    // Add one to the selected day and then get bookings for that day.
    const day = new Date(selectedDay);
    day.setDate(day.getDate() + 1);
    const nextDay = day.toISOString().split('T')[0];
    this.selectedDay.set(nextDay);

    this.getBookingsByDay(nextDay);
  }

  prevDayBookingsByDay() {
    const selectedDay = this.selectedDay();
    if (!selectedDay) {
      return;
    }

    // Subtract one to the selected day and then get bookings for that day.
    const day = new Date(selectedDay);
    day.setDate(day.getDate() - 1);
    const nextDay = day.toISOString().split('T')[0];
    this.selectedDay.set(nextDay);

    this.getBookingsByDay(nextDay);
  }

  nextMonthBookingsByDay() {
    const selectedDay = this.selectedDay();
    if (!selectedDay) {
      return;
    }

    // Add one to the selected day and then get bookings for that day.
    const day = new Date(selectedDay);
    // Advance by one month (preserving the day-of-month when possible)
    day.setMonth(day.getMonth() + 1);
    const nextDay = day.toISOString().split('T')[0];
    this.selectedDay.set(nextDay);

    this.getBookingsByDay(nextDay);
  }

  prevMonthBookingsByDay() {
    const selectedDay = this.selectedDay();
    if (!selectedDay) {
      return;
    }

    const day = new Date(selectedDay);
    day.setMonth(day.getMonth() - 1);
    const nextDay = day.toISOString().split('T')[0];
    this.selectedDay.set(nextDay);

    this.getBookingsByDay(nextDay);
  }
}
