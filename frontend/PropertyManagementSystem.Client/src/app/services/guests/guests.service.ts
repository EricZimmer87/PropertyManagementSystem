import { httpResource, HttpResourceRef } from '@angular/common/http';
import { Service, Signal } from '@angular/core';
import { GuestsResponse } from '../../types/guests/guests-response.type';

@Service()
export class GuestsService {
  url = '/api/guests';

  getGuests(
    pageSize: Signal<number | undefined>,
    pageNumber: Signal<number | undefined>,
    search: Signal<string>,
  ): HttpResourceRef<GuestsResponse | undefined> {
    return httpResource<GuestsResponse>(() => ({
      url: this.url,
      params: {
        pageSize: pageSize() ?? 10,
        pageNumber: pageNumber() ?? 1,
        search: search(),
      },
    }));
  }
}
