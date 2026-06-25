using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;
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
        private readonly IUserStore<AppUser> _userStore;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context,
            IEmailService emailService,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            ILogger<AuthController> logger
            )
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
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

            var result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Errors = result.Errors.Select(e => e.Description)
                });
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
                return BadRequest("Invalid or expired confirmation link.");

            return Ok("Email was successfully confirmed!");
        }

        // TODO ResendEmailConfirmation()

        [HttpGet]
        [Route("login")]
        public async Task<ActionResult> Login()
        {
            return BadRequest("You shall not pass!");
        }
    }
}
