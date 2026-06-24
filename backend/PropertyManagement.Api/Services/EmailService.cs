using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace PropertyManagement.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
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

        public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
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
