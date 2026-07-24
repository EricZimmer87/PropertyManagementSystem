import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BookingsByDayResponse } from './bookings-by-day-response.interface';

@Service()
export class BookingsByDayService {
  private http = inject(HttpClient);
  url = '/api/bookings/by-day';

  getBookingsByDay() {
    return this.http.get<BookingsByDayResponse>(this.url, { withCredentials: true });
  }
}
