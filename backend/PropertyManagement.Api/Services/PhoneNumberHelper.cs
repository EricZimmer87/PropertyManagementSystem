namespace PropertyManagement.Api.Services
{
    public class PhoneNumberHelper
    {
        public static string Normalize(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }
    }
}
