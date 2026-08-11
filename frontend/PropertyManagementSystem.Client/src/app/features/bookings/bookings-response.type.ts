import { Booking } from './booking.type';

export type BookingsResponse = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  items: Booking[];
};
