import { Pipe, PipeTransform } from '@angular/core';
import { BookingStatus, BookingStatusLabels } from '../enums/booking-status.enum';

@Pipe({
  name: 'bookingStatusLabel',
  standalone: true,
})
export class BookingStatusLabelPipe implements PipeTransform {
  transform(value: BookingStatus | number | null | undefined): string {
    if (value === null || value === undefined) {
      return '-';
    }

    return BookingStatusLabels[value as BookingStatus] ?? 'Unknown';
  }
}
