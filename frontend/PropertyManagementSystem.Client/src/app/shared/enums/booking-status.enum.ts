export enum BookingStatus {
  Booked = 0,
  CheckedIn = 1,
  CheckedOut = 2,
  Canceled = 3,
  NoShow = 4,
  Blocked = 5,
}

export const BookingStatusLabels: Record<BookingStatus, string> = {
  [BookingStatus.Booked]: 'Booked',
  [BookingStatus.CheckedIn]: 'Checked In',
  [BookingStatus.CheckedOut]: 'Checked Out',
  [BookingStatus.Canceled]: 'Canceled',
  [BookingStatus.NoShow]: 'No Show',
  [BookingStatus.Blocked]: 'Blocked',
};
