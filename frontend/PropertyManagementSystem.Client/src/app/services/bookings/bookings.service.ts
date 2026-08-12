import { httpResource, HttpResourceRef } from '@angular/common/http';
import { Service, Signal } from '@angular/core';
import { BookingsResponse } from '../../types/bookings/bookings-response.type';

@Service()
export class BookingsService {
  url = '/api/bookings';

  getBookings(
    pageSize: Signal<number | undefined>,
    pageNumber: Signal<number | undefined>,
    search: Signal<string>,
  ): HttpResourceRef<BookingsResponse | undefined> {
    return httpResource<BookingsResponse>(() => ({
      url: this.url,
      params: {
        pageSize: pageSize() ?? 10,
        pageNumber: pageNumber() ?? 1,
        search: search(),
      },
    }));
  }
}
