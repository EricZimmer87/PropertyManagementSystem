import { Unit } from './unit.type';

export type UnitsResponse = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  items: Unit[];
}
