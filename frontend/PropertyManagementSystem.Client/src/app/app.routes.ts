import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Bookings } from './features/bookings/bookings';
import { BookingsByDay } from './features/bookings-by-day/bookings-by-day';
import { Forbidden } from './shared/components/forbidden/forbidden';
import { authGuard } from './auth/auth-guard';
import { adminGuard } from './auth/admin-guard';
import { BookingDetails } from './features/booking-details/booking-details';
import { Guests } from './features/guests/guests';
import { GuestDetails } from './features/guest-details.ts/guest-details';

export const routes: Routes = [
  {
    path: 'forbidden',
    component: Forbidden,
    title: 'Forbidden',
  },
  {
    path: '',
    component: Login,
    title: 'Login',
  },
  {
    path: 'bookings-by-day',
    component: BookingsByDay,
    title: 'Bookings by Day',
    canMatch: [authGuard],
  },
  {
    path: 'bookings/:id',
    component: BookingDetails,
    title: 'Booking Details',
    canMatch: [authGuard],
  },
  {
    path: 'bookings',
    component: Bookings,
    title: 'Bookings',
    canMatch: [authGuard],
  },
  {
    path: 'guests',
    component: Guests,
    title: 'Guests',
    canMatch: [authGuard],
  },
  {
    path: 'guests/:id',
    component: GuestDetails,
    title: 'Guest Details',
    canMatch: [authGuard],
  },
];
