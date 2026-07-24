import { BookingStatus } from '../../shared/enums/booking-status.enum';

export interface BookingsByDayResponse {
  CreatedOn: Date;
  CreatedByUserName?: string;
  UnitNumber: string;
  GuestName: string;
  Address?: string;
  City?: string;
  State?: string;
  ZipCode?: string;
  PhoneNumber?: string;
  StartDate: Date;
  EndDate: Date;
  Occupants?: number;
  Status: BookingStatus;
  Notes?: string;
  CardLastFour?: number;
}
