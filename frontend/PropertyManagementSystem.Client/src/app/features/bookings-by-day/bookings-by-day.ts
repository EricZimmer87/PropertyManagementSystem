import { Component, computed, inject, signal } from '@angular/core';
import { BookingsByDayService } from './bookings-by-day.service';
import { DatePipe } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BookingStatusLabelPipe } from '../../shared/pipes/booking-status-label.pipe';
import { BookingStatus } from '../../shared/enums/booking-status.enum';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-bookings-by-day',
  imports: [DatePipe, ReactiveFormsModule, RouterLink, BookingStatusLabelPipe],
  templateUrl: './bookings-by-day.html',
  styleUrl: './bookings-by-day.css',
})
export class BookingsByDay {
  public readonly BookingStatus = BookingStatus;
  private readonly bookingsByDayService = inject(BookingsByDayService);

  selectDayForm = new FormGroup({
    selectedDay: new FormControl<string | null>(null),
  });

  // If requestedDay changes, it fetches updated bookings.
  private readonly requestedDay = signal<string | undefined>(undefined);

  readonly bookingsResource = this.bookingsByDayService.getBookingsByDay(this.requestedDay);

  readonly bookings = computed(() =>
    this.bookingsResource.hasValue() ? this.bookingsResource.value().bookings : [],
  );

  readonly selectedDay = computed(() =>
    this.bookingsResource.hasValue() ? this.bookingsResource.value().selectedDay : undefined,
  );

  readonly isLoading = this.bookingsResource.isLoading;

  readonly errorMessage = computed(() => {
    const error = this.bookingsResource.error();

    if (error instanceof HttpErrorResponse) {
      return error.error?.message ?? error.message;
    }

    return error?.message ?? null;
  });
  readonly doubleBookedUnits = computed(() =>
    this.bookingsByDayService.findDoubleBookedUnits(this.bookings()),
  );

  onSubmitSelectedDay() {
    const selectedDay = this.selectDayForm.value.selectedDay;

    if (!selectedDay) {
      return;
    }

    if (selectedDay === this.requestedDay()) {
      this.bookingsResource.reload();
    } else {
      this.requestedDay.set(selectedDay);
    }
  }

  addDay(amount: number) {
    const selectedDay = this.selectedDay();
    if (!selectedDay) return;

    const day = new Date(`${selectedDay}T00:00:00`);
    day.setDate(day.getDate() + amount);

    this.requestedDay.set(this.formatDate(day));
  }

  addMonth(amount: number) {
    const selectedDay = this.selectedDay();
    if (!selectedDay) return;

    const day = new Date(`${selectedDay}T00:00:00`);
    const originalDay = day.getDate();

    // Prevent overflow while moving months
    day.setDate(1);
    day.setMonth(day.getMonth() + amount);

    // Last valid day of the target month
    const lastDay = new Date(day.getFullYear(), day.getMonth() + 1, 0).getDate();

    day.setDate(Math.min(originalDay, lastDay));

    this.requestedDay.set(this.formatDate(day));
  }

  private formatDate(day: Date): string {
    const year = day.getFullYear();
    const month = String(day.getMonth() + 1).padStart(2, '0');
    const date = String(day.getDate()).padStart(2, '0');

    return `${year}-${month}-${date}`;
  }

  // nextMonthBookingsByDay() {
  //   const selectedDay = this.selectedDay();
  //   if (!selectedDay) {
  //     return;
  //   }

  //   // Add one to the selected day and then get bookings for that day.
  //   const day = new Date(selectedDay);
  //   // Advance by one month (preserving the day-of-month when possible)
  //   day.setMonth(day.getMonth() + 1);
  //   const nextDay = day.toISOString().split('T')[0];
  //   this.selectedDay.set(nextDay);

  //   this.getBookingsByDay(nextDay);
  // }

  // prevMonthBookingsByDay() {
  //   const selectedDay = this.selectedDay();
  //   if (!selectedDay) {
  //     return;
  //   }

  //   const day = new Date(selectedDay);
  //   day.setMonth(day.getMonth() - 1);
  //   const nextDay = day.toISOString().split('T')[0];
  //   this.selectedDay.set(nextDay);

  //   this.getBookingsByDay(nextDay);
  // }
}
