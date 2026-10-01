import { inject, Service } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Service()
export class DeleteAllowedEmailService {
  http = inject(HttpClient);
  private readonly url = '/api/allowed-emails';

  public deleteAllowedEmail(id: number) {
    return this.http.delete<any>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to delete allowed email.'));
      })
    )
  }
}
