import { Component, computed, DestroyRef, inject, signal } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { BookingByDay } from './booking-by-day.type';
import { BookingsByDayService } from './bookings-by-day.service';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { BookingsByDayResponse } from './bookings-by-day-response.type';

@Component({
  selector: 'app-bookings-by-day',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay {
  bookingsByDayService = inject(BookingsByDayService);
  destroyRef = inject(DestroyRef);

  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  bookings = signal<BookingByDay[]>([]);
  doubleBookedUnits = signal<Set<string>>(new Set());

  selectedDay = signal<string | null>(null);

  selectDayForm = new FormGroup({
    selectedDay: new FormControl(''),
  });

  // Add a computed signal
  unitsWithMultipleBookings = computed(() => {
    const counts = new Map<string, number>();
    for (const b of this.bookings()) {
      if (b.bookingId !== 0) {
        counts.set(b.unitNumber, (counts.get(b.unitNumber) ?? 0) + 1);
      }
    }
    return new Set([...counts.entries()].filter(([_, count]) => count > 1).map(([unit]) => unit));
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

          // Checks for double-booked units
          const seen = new Set<string>();
          const doubles = new Set<string>();
          for (const b of data.bookings) {
            if (b.bookingId === 0) continue;
            if (seen.has(b.unitNumber)) doubles.add(b.unitNumber);
            else seen.add(b.unitNumber);
          }
          this.doubleBookedUnits.set(doubles);

          this.selectDayForm.patchValue({ selectedDay: data.selectedDay });
          this.selectedDay.set(data.selectedDay);
          this.isLoading.set(false);
          this.errorMessage.set(null);
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

    // Add one to the selected day and then get bookings for that day.
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
