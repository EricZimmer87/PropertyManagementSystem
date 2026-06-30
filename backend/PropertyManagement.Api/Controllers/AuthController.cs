using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Auth;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services.Email;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(AppDbContext context,
            IEmailService emailService,
            UserManager<AppUser> userManager,
            ILogger<AuthController> logger,
            SignInManager<AppUser> signInManager
            )
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
            _signInManager = signInManager;
        }

        // POST /api/Auth/register - registers a new user, if their email is allowed
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
                return IdentityValidationProblem(result);
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            try
            {
                await _emailService.SendRegisterConfirmEmailAsync(
                    user.Email,
                    user);
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

        // POST /api/Auth/resend-confirmation/email - resends the confirmation email
        [HttpPost("resend-confirmation-email")]
        public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {
            var email = request.Email.Trim();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("No account exists for that email.");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email has already been confirmed.");

            try
            {
                await _emailService.SendRegisterConfirmEmailAsync(
                    user.Email!,
                    user);
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

        // GET /api/Auth/confirm-email - confirms email address from the confirmation email link
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
                return IdentityValidationProblem(result);

            return Ok(new
            {
                Message = "Email was successfully confirmed!"
            });
        }

        // POST /api/Auth/login - logs the user in
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

        // DELETE /api/Auth/logout - logs the user out
        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User {UserId} logged out.", _userManager.GetUserId(User));

            return NoContent();
        }

        // POST /api/Auth/forgot-password - sends a link to the email address to reset password
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var requestEmail = request.Email.Trim();
            // Check if the email even exists as a user
            var user = await _userManager.FindByEmailAsync(requestEmail);
            // User with the requested email does not exist, so just send that the email has been sent to not reveal
            // whether there is a user by that email.
            if (user == null) return Ok("Password reset link has been sent. Please check your email.");

            try
            {
                await _emailService.SendPasswordResetLinkAsync(requestEmail, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send password reset email to {Email}",
                    requestEmail);

                return StatusCode(500,
                    "We couldn't send the password reset email.");
            }

            return Ok("Password reset link has been sent. Please check your email.");
        }

        // POST /api/Auth/reset-password - resets the user's password
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var requestEmail = request.Email.Trim();
            var user = await _userManager.FindByEmailAsync(requestEmail);
            if (user == null) return BadRequest("Unable to reset password.");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            return Ok(new
            {
                Message = "Password has been reset successfully."
            });
        }

        // POST /api/Auth/change-password - change user's password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            return Ok(new
            {
                Message = "Password has successfully been changed."
            });
        }
    }
}
