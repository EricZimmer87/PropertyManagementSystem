import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { BookingsByDayResponse } from './bookings-by-day-response.type';

@Service()
export class BookingsByDayService {
  private http = inject(HttpClient);
  url = '/api/bookings/by-day';

  getBookingsByDay(params?: HttpParams): Observable<BookingsByDayResponse> {
    return this.http.get<BookingsByDayResponse>(this.url, { params }).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch bookings.'));
      }),
    );
  }
}
