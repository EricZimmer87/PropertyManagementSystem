import { inject, Service } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { GuestDetailsResponse } from '../../types/guests/guest-details-response.type';

@Service()
export class GetGuestByIdService {
  private http = inject(HttpClient);
  url = '/api/guests';

  getGuest(id: number): Observable<GuestDetailsResponse> {
    return this.http.get<GuestDetailsResponse>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch guest.'));
      })
    )
  }
}
