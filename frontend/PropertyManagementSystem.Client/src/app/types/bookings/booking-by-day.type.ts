import { BookingStatus } from '../../enums/booking-status.enum';

export type BookingByDay = {
  bookingId: number | null;
  createdOn: string | null;
  createdByUserName?: string | null;
  unitNumber: string;
  guestName: string | null;
  address?: string | null;
  city?: string | null;
  state?: string | null;
  zipCode?: string | null;
  phoneNumber?: string | null;
  startDate: string | null;
  endDate: string | null;
  occupants?: number | null;
  status: BookingStatus | null;
  notes?: string | null;
  cardLastFour?: number | null;
};
