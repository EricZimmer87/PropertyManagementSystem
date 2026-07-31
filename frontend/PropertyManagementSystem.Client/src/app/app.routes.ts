import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { BookingsByDay } from './features/bookings-by-day/bookings-by-day';
import { Forbidden } from './shared/components/forbidden/forbidden';
import { authGuard } from './auth/auth-guard';
import { adminGuard } from './auth/admin-guard';

export const routes: Routes = [
  {
    path: 'forbidden',
    component: Forbidden,
    title: 'Forbidden',
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
    canMatch: [authGuard],
  },
];
