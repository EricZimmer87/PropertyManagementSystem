import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BookingByDay } from './booking-by-day.type';
import { catchError, Observable, throwError } from 'rxjs';

@Service()
export class BookingsByDayService {
  private http = inject(HttpClient);
  url = '/api/bookings/by-day';

  getBookingsByDay(): Observable<BookingByDay[]> {
    return this.http.get<BookingByDay[]>(this.url).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch bookings.'));
      }),
    );
  }
}
