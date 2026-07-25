import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { BookingsByDay } from './features/bookings-by-day/bookings-by-day';
import { Unauthenticated } from './features/auth/unauthenticated/unauthenticated';
import { Unauthorized } from './features/auth/unauthorized/unauthorized';

export const routes: Routes = [
  {
    path: 'unauthenticated',
    component: Unauthenticated,
    title: 'Unauthenticated',
  },
  {
    path: 'unauthorized',
    component: Unauthorized,
    title: 'Unauthorized',
  },
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
