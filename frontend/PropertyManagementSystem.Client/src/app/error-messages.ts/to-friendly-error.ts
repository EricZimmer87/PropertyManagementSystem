export function toFriendlyError(code: string | null): string | null {
  // This function can now be called on a null value,
  // and it will return null instead of throwing an error.
  if (!code) return null;

  switch (code) {
    case 'not_logged_in':
      return 'Please log in to continue.';
    case 'forbidden':
      return 'You do not have permission to access this page.';
    case 'auth_failed':
      return 'Login failed. Please check your credentials and try again.';
    case 'external_auth_failed':
      return 'External authentication failed.';
    case 'failed_to_create_user':
      return 'Failed to create user.';
    case 'account_deactivated':
      return 'Your account has been deactivated.';
    case 'site_access_forbidden':
      return 'You are not allowed access to this site.';
    default:
      return 'Login error.';
  }
}
