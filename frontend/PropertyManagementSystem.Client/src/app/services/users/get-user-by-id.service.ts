import { inject, Service } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { UserDetailsResponse } from '../../types/users/user-details-response.type';

@Service()
export class GetUserByIdService {
  private http = inject(HttpClient);
  url = '/api/users';

  getUser(id: string): Observable<UserDetailsResponse> {
    return this.http.get<UserDetailsResponse>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch user.'));
      }),
    );
  }
}
