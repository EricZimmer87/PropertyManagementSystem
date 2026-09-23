import { Service, Signal } from '@angular/core';
import { httpResource, HttpResourceRef } from '@angular/common/http';
import { UsersResponse } from '../../types/users/users-response.type';

@Service()
export class UsersService {
  url = '/api/users';

  getUsers(
    pageSize: Signal<number | undefined>,
    pageNumber: Signal<number | undefined>,
    search: Signal<string>,
    sort: Signal<string>,
  ): HttpResourceRef<UsersResponse | undefined> {
    return httpResource<UsersResponse>(() => ({
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
