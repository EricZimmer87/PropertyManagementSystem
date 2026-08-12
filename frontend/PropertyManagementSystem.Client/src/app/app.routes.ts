import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Bookings } from './components/bookings/bookings';
import { BookingsByDay } from './components/bookings-by-day/bookings-by-day';
import { Forbidden } from './components/forbidden/forbidden';
import { authGuard } from './auth/auth-guard';
import { adminGuard } from './auth/admin-guard';
import { BookingDetails } from './components/booking-details/booking-details';
import { Guests } from './components/guests/guests';
import { GuestDetails } from './components/guest-details.ts/guest-details';

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
