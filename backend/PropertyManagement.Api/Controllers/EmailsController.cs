using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Api.Services.Email;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailsController> _logger;

        public EmailsController(IEmailService emailService, ILogger<EmailsController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(SendEmailRequest request)
        {
            try
            {
                await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);
                return Ok("Email sent successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email.");

                return StatusCode(500, "Unable to send email.");
            }
        }
    }

    public class SendEmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
