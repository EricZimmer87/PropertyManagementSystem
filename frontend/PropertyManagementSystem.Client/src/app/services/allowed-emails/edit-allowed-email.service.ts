import { inject, Service } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AllowedEmailsResponse } from '../../types/allowed-emails/allowed-emails-response.type';
import { catchError, Observable, throwError } from 'rxjs';
import { AllowedEmailDetailsResponse } from '../../types/allowed-emails/allowed-email-details-response.type';
import { AllowedEmailEditRequest } from '../../types/allowed-emails/allowed-email-edit-request.type';

@Service()
export class EditAllowedEmailService {
  private http = inject(HttpClient);
  private url = '/api/allowed-emails';

  editAllowedEmail(id: number, request: AllowedEmailEditRequest): Observable<AllowedEmailDetailsResponse> {
    return this.http.patch<AllowedEmailDetailsResponse>(`${this.url}/${id}`, request).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch allowed email.'));
      }),
    );
  }
}
