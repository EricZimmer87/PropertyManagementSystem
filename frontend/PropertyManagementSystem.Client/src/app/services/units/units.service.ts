import { Service, Signal } from '@angular/core';
import { UnitsResponse } from '../../types/units/units-response.type';
import { httpResource, HttpResourceRef } from '@angular/common/http';

@Service()
export class UnitsService {
  url = '/api/units';

  getUnits(
    pageSize: Signal<number | undefined>,
    pageNumber: Signal<number | undefined>,
    search: Signal<string>,
    sort: Signal<string>,
  ): HttpResourceRef<UnitsResponse | undefined> {
    return httpResource<UnitsResponse>(() => ({
      url: this.url,
      params: {
        pageSize: pageSize() ?? 10,
        pageNumber: pageNumber() ?? 1,
        search: search(),
        sort: sort()
      },
    }));
  }
}
