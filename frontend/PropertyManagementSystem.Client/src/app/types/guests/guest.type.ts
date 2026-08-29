export type Guest = {
  guestId: number;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  normalizedPhoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode: string;
  email?: string;
  company?: string;
  notes?: string;
};
