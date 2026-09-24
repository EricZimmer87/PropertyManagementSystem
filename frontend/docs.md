# Interceptors

## Credentials Interceptor

- Adds `withCredentials: true` to each HTTP request to send cookie data to backend

# Auth Service

- Uses Signals to keep current user data up to date
- Signals include `currentUser` (whether a user is logged in) and `isAdmin` (whether the user is an admin)
- The `Login()` and `Logout()` methods update Signals
- `loadCurrentUser()` method makes a call to the backend to ensure updated data about the current user
  - Executes upon app initialization (`app.config.ts`) to keep Signals up to date if user refreshes the page in the browser

# Guards

- Kept simple via using signals from `auth.service.ts`

# Pagination

- Reusable component whenever pagination is needed
- Handles most logic for pagination
  - `changePage()` and `changePageSize()` are left in the components to more easily handle updated signal values
