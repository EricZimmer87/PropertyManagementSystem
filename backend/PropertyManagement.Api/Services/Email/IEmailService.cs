using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendRegisterConfirmEmailAsync(string toEmail, AppUser user);
        Task SendChangeEmailConfirmAsync(string toEmail, AppUser user);
        Task SendPasswordResetLinkAsync(string email, AppUser user);
    }
}