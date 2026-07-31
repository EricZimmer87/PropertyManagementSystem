import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BookingsByDayResponse } from './bookings-by-day-response.interface';
import { catchError, throwError } from 'rxjs';

@Service()
export class BookingsByDayService {
  private http = inject(HttpClient);
  url = '/api/bookings/by-day';

  getBookingsByDay() {
    return this.http.get<BookingsByDayResponse>(this.url).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;

        return throwError(() => new Error(backendMessage || 'Failed to fetch bookings.'));
      }),
    );
  }
}
