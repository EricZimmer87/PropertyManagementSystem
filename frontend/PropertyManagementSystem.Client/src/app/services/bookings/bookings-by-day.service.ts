import { Service, Signal, inject } from '@angular/core';
import { httpResource } from '@angular/common/http';
import { BookingsByDayResponse } from '../../types/bookings/bookings-by-day-response.type';
import { BookingByDay } from '../../types/bookings/booking-by-day.type';

@Service()
export class BookingsByDayService {
  url = '/api/bookings/by-day';

  getBookingsByDay(selectedDay: Signal<string | undefined>) {
    return httpResource<BookingsByDayResponse>(() => ({
      url: this.url,
      params: selectedDay() ? { selectedDay: selectedDay()! } : undefined,
    }));
  }

  findDoubleBookedUnits(bookings: BookingByDay[]): Set<string> {
    const seen = new Set<string>();
    const doubles = new Set<string>();

    for (const b of bookings) {
      if (b.bookingId === 0) continue;
      if (seen.has(b.unitNumber)) doubles.add(b.unitNumber);
      else seen.add(b.unitNumber);
    }
    return doubles;
  }
}
