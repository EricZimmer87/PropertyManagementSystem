import { Guest } from './guest-type';

export type GuestsResponse = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  items: Guest[];
};
