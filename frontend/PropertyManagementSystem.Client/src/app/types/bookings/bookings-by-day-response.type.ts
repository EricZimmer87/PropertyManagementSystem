import { BookingByDay } from './booking-by-day.type';

export type BookingsByDayResponse = {
  selectedDay: string;
  bookings: BookingByDay[];
};
