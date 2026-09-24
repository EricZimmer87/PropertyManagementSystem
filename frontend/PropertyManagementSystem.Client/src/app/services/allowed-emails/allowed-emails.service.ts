import { Service, Signal } from '@angular/core';
import { httpResource, HttpResourceRef } from '@angular/common/http';
import { AllowedEmailsResponse } from '../../types/allowed-emails/allowed-emails-response.type';

@Service()
export class AllowedEmailsService {
  url = '/api/allowed-emails';

  getAllowedEmails(
    pageSize: Signal<number | undefined>,
    pageNumber: Signal<number | undefined>,
    search: Signal<string>,
    sort: Signal<string>,
  ): HttpResourceRef<AllowedEmailsResponse | undefined> {
    return httpResource<AllowedEmailsResponse>(() => ({
      url: this.url,
      params: {
        pageSize: pageSize() ?? 10,
        pageNumber: pageNumber() ?? 1,
        search: search(),
        sort: sort(),
      },
    }));
  }
}
