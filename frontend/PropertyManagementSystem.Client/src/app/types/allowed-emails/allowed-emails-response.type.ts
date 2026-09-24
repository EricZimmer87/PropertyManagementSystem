import { AllowedEmail } from './allowed-emails.type';

export type AllowedEmailsResponse = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  items: AllowedEmail[];
};
