using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Auth;
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
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(AppDbContext context,
            IEmailService emailService,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            ILogger<AuthController> logger,
            SignInManager<AppUser> signInManager
            )
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        // Adapted from ASP.NET Core Identity's IdentityApiEndpointRouteBuilderExtensions.
        // Converts IdentityResult errors into a ValidationProblemDetails response.
        // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs
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

        private async Task<string> GenerateEmailConfirmationLinkAsync(AppUser user)
        {
            // Generate a one-time email confirmation token.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            // Change for correct front end later
            var apiUrl = _configuration["AppSettings:ApiUrl"];
            return
                $"{apiUrl}/Auth/confirm-email?userId={user.Id}&token={encodedToken}";

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserRequest registration)
        {
            var email = registration.Email.Trim();
            var normalizedEmail = _userManager.NormalizeEmail(email);
            var firstName = registration.FirstName.Trim();
            var lastName = registration.LastName.Trim();

            // Check if registration email is allowed
            var allowed = await _context.AllowedEmails
                .AnyAsync(a => a.NormalizedEmail == normalizedEmail);
            if (!allowed)
                return BadRequest("Registration is not allowed for this email address.");

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
            {
                return BadRequest("An account with this email already exists.");
            }

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            var confirmationLink = await GenerateEmailConfirmationLinkAsync(user);

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
                    "Your account was created, but we couldn't send the confirmation email. " +
                    "Please try the 'Resend confirmation email' option.");
            }

            return Ok(new
            {
                Message = "Registration successful. Please check your email to confirm your account."
            });
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {
            var email = request.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("No account exists for that email.");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email has already been confirmed.");

            var confirmationLink = await GenerateEmailConfirmationLinkAsync(user);

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
                    email);

                return StatusCode(500,
                    "We couldn't send the confirmation email. " +
                    "Please try the 'Resend confirmation email' option.");
            }

            return Ok("Please check your email to confirm your account.");
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
                return Ok("Email has already been confirmed.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return CreateValidationProblem(result);

            return Ok(new
            {
                Message = "Email was successfully confirmed!"
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            // Ensure user exists
            var email = request.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized("Email or password is incorrect.");

            // Ensure email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Please confirm your email before logging in.");

            // Check if account is active
            if (!user.IsActive)
                return Unauthorized("Your account is no longer active.");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return Unauthorized("Your account is locked.");

            if (!result.Succeeded)
                return Unauthorized("Email or password is incorrect.");

            _logger.LogInformation("User {Email} logged in at {Time}.",
                user.Email, DateTime.UtcNow);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User {UserId} logged out.", _userManager.GetUserId(User));

            return NoContent();
        }

        // TODO Forgot Password

        // TODO Reset Password

        // TODO Change Password

        // TODO Change Email

        // TODO Change First Name

        // TODO Change Last Name

        // TODO Fix this to use User ID, not email, and put it in an AppUsersController
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("users/{id}/active")]
        public async Task<ActionResult> SetIsActive(IsActiveRequest request)
        {
            // Ensure user exists
            var email = request.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized("Email or password is incorrect.");

            user.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"{user.Email}'s active status has been set to {user.IsActive}."
            });
        }
    }
}
