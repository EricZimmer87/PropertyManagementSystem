# Authentication

- Using ASP.NET Identity for user and role management
- Default Identity settings configured in `Program.cs`
- `AppUser` custom entity that extends `IdentityUser`
- Custom auth endpoints for app-specific business rules

# Database

- `UnitNumber` in `Units` table must be unique to prevent duplicate records and bookings
- Guests' `NormalizedPhoneNumber` must be unique to provide unique identification for guests
- Audit-user relationships are explicitly configured without cascade delete
  - Users cannot be deleted while referenced by bookings to keep historical records accurate
- `SaveChanges()` and `SaveChangesAsync()` overrides normalize guest phone numbers and allowed emails automatically in the `AppDbContext` to prevent duplicate guests and users

# Database Seeding

- Database is populated with fields to provide sample data for development testing
- Admin user should be configured in `production.appsettings.json` to provide initial admin user

# Services

## Email

- Email is configured using SMTP and using `NetCore.MailKit` package
- SMTP settings are in `appsettings.json`
- `EmailHelper` adds helper functions, such as `IsValidEmail` and `Normalize`.
- `EmailService` class contains methods for sending emails and is registered in `Program.cs` for dependency injection

## PhoneNumberHelper

- Normalizes phone numbers

# Controllers

## BaseApiController

- `IdentityValidationProblem` was adapted from ASP.NET Identity's `IdentityApiEndpointRouteBuilderExtensions` to convert `IdentityResult` errors into a `ValidationProblemDetails` response for more consistent error logging.

## Auth Controller

- `AuthController` handles authentication API end points.
- Inherits from `BaseApiController` to use `IdentityValidationProblem`

### POST /register

Creates a new user account

- Checks the `AllowedEmails` table before permitting user to create account
  - An allowed email can only be added/removed by admin account
- A new `AppUser` is created upon success
- Default `Role` is set to `User`
- Sends email confirmation link
- Email must be confirmed before login is permitted

## Users Controller

- Inherits from `BaseApiController` to use `IdentityValidationProblem`
