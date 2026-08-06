import { Service, Signal, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams, httpResource } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { BookingsByDayResponse } from './bookings-by-day-response.type';
import { BookingByDay } from './booking-by-day.type';

@Service()
export class BookingsByDayService {
  private http = inject(HttpClient);
  url = '/api/bookings/by-day';

  getBookingsByDay(selectedDay: Signal<string | undefined>) {
    return httpResource<BookingsByDayResponse>(() => ({
      url: this.url,
      params: selectedDay() ? { selectedDay: selectedDay()! } : undefined,
    }));
  }

  /* user = httpResource(() => ({
  url: `/api/user/${userId()}`,
  method: 'GET',
  headers: {
    'X-Special': 'true',
  },
  params: {
    'fast': 'yes',
  },
  reportProgress: true,
  transferCache: true,
  keepalive: true,
  mode: 'cors',
  redirect: 'error',
  priority: 'high',
  cache: 'force-cache',
  credentials: 'include',
  referrer: 'no-referrer',
  integrity: 'sha384-oqVuAfXRKap7fdgcCY5uykM6+R9GhEXAMPLEKEY=',
  referrerPolicy: 'no-referrer',
}));
*/

  // return this.http.get<BookingsByDayResponse>(this.url, { params }).pipe(
  //   catchError((err: HttpErrorResponse) => {
  //     const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
  //     return throwError(() => new Error(backendMessage || 'Failed to fetch bookings.'));
  //   }),
  // );

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
