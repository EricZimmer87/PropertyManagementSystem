using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;
using System.Diagnostics;
using System.Net;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context,
            IEmailService emailService,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            ILogger<AuthController> logger
            )
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        private ActionResult CreateValidationProblem(IdentityResult result)
        {
            // We expect a single error code and description in the normal case.
            // This could be golfed with GroupBy and ToDictionary, but perf! :P
            Debug.Assert(!result.Succeeded);
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return ValidationProblem(new ValidationProblemDetails(errorDictionary));
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest registration)
        {
            var email = registration.Email;

            // Check for valid email address
            if (string.IsNullOrEmpty(email) || !EmailHelper.IsValidEmail(email))
            {
                return BadRequest(_userManager.ErrorDescriber.InvalidEmail(email));
            }

            var normalizedEmail = _userManager.NormalizeEmail(email);

            // Check if registration email is allowed
            var allowed = await _context.AllowedEmails
                .AnyAsync(a => a.NormalizedEmail == normalizedEmail);
            if (!allowed)
            {
                return BadRequest("Registration is not allowed for this email address.");
            }

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
            {
                return BadRequest("An account with this email already exists.");
            }

            var user = new AppUser
            {
                UserName = email,
                Email = email
            };

            IdentityResult result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            // Generate a one-time email confirmation token.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var frontendUrl = _configuration["AppSettings:FrontendUrl"];
            var confirmationLink =
                $"{frontendUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            try
            {
                await _emailService.SendConfirmationEmailAsync(
                    user.Email!,
                    confirmationLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send confirmation email to {Email}",
                    user.Email);

                return StatusCode(500,
                    "Your account was created, but we couldn't send the confirmation email.");
            }

            return Ok(new
            {
                Message = "Registration successful. Please check your email to confirm your account."
            });
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid confirmation link.");
            }

            // Get user from userID
            var user = await _userManager.FindByIdAsync(userId);

            // Ensure user exists
            if (user == null)
                return BadRequest("Invalid confirmation link.");

            // Check if email has already been confirmed
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return Ok("Email has already been confirmed.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return CreateValidationProblem(result);

            return Ok("Email was successfully confirmed!");
        }

        // TODO ResendEmailConfirmation()

        // TODO
        [HttpGet]
        [Route("login")]
        public async Task<ActionResult> Login()
        {
            return BadRequest("You shall not pass!");
        }
    }
}
