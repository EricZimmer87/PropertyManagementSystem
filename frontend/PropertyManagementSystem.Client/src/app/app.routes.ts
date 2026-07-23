import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { BookingsByDay } from './features/bookings-by-day/bookings-by-day';

export const routes: Routes = [
  {
    path: 'login',
    component: Login,
    title: 'Login',
  },
  {
    path: 'bookings-by-day',
    component: BookingsByDay,
    title: 'Bookings by Day',
  },
];
