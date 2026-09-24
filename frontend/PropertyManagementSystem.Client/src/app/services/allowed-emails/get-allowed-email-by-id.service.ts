import { inject, Service } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { AllowedEmailDetailsResponse } from '../../types/allowed-emails/allowed-email-details-response.type';

@Service()
export class GetAllowedEmailByIdService {
  private http = inject(HttpClient);
  url = '/api/allowed-emails';

  getAllowedEmail(id: number): Observable<AllowedEmailDetailsResponse> {
    return this.http.get<AllowedEmailDetailsResponse>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch allowed email.'));
      }),
    );
  }
}
