import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Bookings } from './components/bookings/bookings';
import { BookingsByDay } from './components/bookings-by-day/bookings-by-day';
import { Forbidden } from './components/forbidden/forbidden';
import { authGuard } from './auth/auth-guard';
import { adminGuard } from './auth/admin-guard';
import { BookingDetails } from './components/booking-details/booking-details';
import { Guests } from './components/guests/guests';
import { GuestDetails } from './components/guest-details/guest-details';
import { Units } from './components/units/units/units';
import { UnitDetails } from './components/unit-details/unit-details';
import { Users } from './components/users/users/users';
import { UserDetails } from './components/user-details/user-details/user-details';
import { AllowedEmails } from './components/allowed-emails/allowed-emails/allowed-emails';
import { AllowedEmailDetails } from './components/allowed-emails/allowed-email-details/allowed-email-details';
import { AllowedEmailsEdit } from './components/allowed-emails/allowed-emails-edit/allowed-emails-edit';
import { AllowedEmailsCreate } from './components/allowed-emails/allowed-emails-create/allowed-emails-create';
import { AllowedEmailsDelete } from './components/allowed-emails/allowed-emails-delete/allowed-emails-delete';

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
  {
    path: 'units',
    component: Units,
    title: 'Units',
    canMatch: [authGuard],
  },
  {
    path: 'units/:id',
    component: UnitDetails,
    title: 'Unit Details',
    canMatch: [authGuard],
  },
  {
    path: 'users',
    component: Users,
    title: 'Users',
    canMatch: [adminGuard],
  },
  {
    path: 'users/:id',
    component: UserDetails,
    title: 'User Details',
    canMatch: [adminGuard],
  },
  {
    path: 'allowed-emails',
    children: [
      {
        path: '',
        component: AllowedEmails,
        title: 'Allowed Emails',
        canMatch: [adminGuard],
      },
      {
        path: 'create',
        component: AllowedEmailsCreate,
        title: 'Add Allowed Email',
        canMatch: [adminGuard],
      },
      {
        path: 'delete/:id',
        component: AllowedEmailsDelete,
        title: 'Delete Allowed Email',
        canMatch: [adminGuard],
      },
      {
        path: ':id',
        component: AllowedEmailDetails,
        title: 'Allowed Email Details',
        canMatch: [adminGuard],
      },
      {
        path: 'edit/:id',
        component: AllowedEmailsEdit,
        title: 'Edit Allowed Email',
        canMatch: [adminGuard],
      },
    ],
  },
];
