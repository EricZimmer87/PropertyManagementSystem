using Microsoft.AspNetCore.Identity;
using PropertyManagement.Api.Models;
using System.Net;
using System.Net.Mail;

namespace PropertyManagement.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public EmailService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var server = smtpSettings["Server"];
            var port = int.Parse(smtpSettings["Port"]!);
            var senderEmail = smtpSettings["SenderEmail"]!;
            var senderName = smtpSettings["SenderName"];
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];

            using var client = new SmtpClient(server, port);
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }

        public async Task SendRegisterConfirmEmailAsync(string toEmail, AppUser user)
        {
            // Generate a one-time email confirmation token.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            // Change for correct front end later
            var apiUrl = _configuration["AppSettings:ApiUrl"];
            var confirmationLink =
                $"{apiUrl}/Auth/confirm-email?userId={user.Id}&token={encodedToken}";

            var body = $"""
                Please confirm your email address.

                <a href="{confirmationLink}">
                    Confirm Email
                </a>
                """;

            await SendEmailAsync(
                toEmail,
                "Confirm your email address",
                body);
        }

        public async Task SendChangeEmailConfirmAsync(string toEmail, AppUser user)
        {
            // Generate a one-time email confirmation token.
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, toEmail);
            var encodedToken = WebUtility.UrlEncode(token);
            // Change for correct front end later
            var apiUrl = _configuration["AppSettings:ApiUrl"];
            var confirmationLink =
                $"{apiUrl}/Users/change-email-confirm?newEmail={toEmail}&userId={user.Id}&token={encodedToken}";

            var body = $"""
                Please confirm your email address.

                <a href="{confirmationLink}">
                    Confirm Email
                </a>
                """;

            await SendEmailAsync(
                toEmail,
                "Confirm your email address",
                body);
        }
    }
}
