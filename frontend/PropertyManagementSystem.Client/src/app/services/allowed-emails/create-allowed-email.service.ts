import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AllowedEmailCreateRequest } from '../../types/allowed-emails/allowed-email-create-request';
import { Observable } from 'rxjs';
import { AllowedEmailCreateResponse } from '../../types/allowed-emails/allowed-email-create-response';

@Service()
export class CreateAllowedEmailService {
  http = inject(HttpClient);

  createUrl = '/api/allowed-emails/add';

  createAllowedEmail(request: AllowedEmailCreateRequest): Observable<AllowedEmailCreateResponse> {
    return this.http.post<AllowedEmailCreateResponse>(this.createUrl, request);
  }
}
