import { BookingStatus } from '../../shared/enums/booking-status.enum';

export type BookingByDay = {
  bookingId: number;
  createdOn: string;
  createdByUserName?: string;
  unitNumber: string;
  guestName: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  phoneNumber?: string;
  startDate: string;
  endDate: string;
  occupants?: number;
  status: BookingStatus;
  notes?: string;
  cardLastFour?: number | null;
};
