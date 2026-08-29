import { inject, Service } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { UnitDetailsResponse } from '../../types/units/unit-details-response.type';

@Service()
export class GetUnitByIdService {
  private http = inject(HttpClient);
  url = '/api/units';

  getUnit(id: number): Observable<UnitDetailsResponse> {
    return this.http.get<UnitDetailsResponse>(`${this.url}/${id}`).pipe(
      catchError((err: HttpErrorResponse) => {
        const backendMessage = typeof err.error === 'string' ? err.error : err.error?.message;
        return throwError(() => new Error(backendMessage || 'Failed to fetch unit.'));
      })
    )
  }
}
