import { BookingStatus } from '../enums/booking-status.enum';

export type BookingDetailsResponse = {
  bookingId: number;
  guestName: string;
  unitNumber: string;
  startDate: string;
  endDate: string;
  occupants?: number;
  status: BookingStatus;
  notes?: string;
  cardLastFour?: number;
  createdOn: string;
  modifiedOn?: string;
  canceledOn?: string;
  createdByUserName?: string;
  modifiedByUserName?: string;
  canceledByUserName?: string;
};
