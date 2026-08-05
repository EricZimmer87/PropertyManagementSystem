import { Service, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BookingDetailsResponse } from '../types/booking-details-response.type';
import { catchError, Observable, throwError } from 'rxjs';

@Service()
export class GetBookingByIdService {
  private http = inject(HttpClient);
  url = '/api/bookings';

  getBooking(id: number): Observable<BookingDetailsResponse> {
    return this.http.get<BookingDetailsResponse>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch booking.'));
      }),
    );
  }
}
