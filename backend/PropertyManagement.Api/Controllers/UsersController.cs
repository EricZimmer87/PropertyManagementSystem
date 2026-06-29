using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.DTOs.AppUsers;
using PropertyManagement.Api.DTOs.Users;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public UsersController(UserManager<AppUser> userManager,
            IEmailService emailService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        // GET /users - gets all users
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var users = await _userManager
                .Users
                .AsNoTracking()
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            var response = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                response.Add(new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault()
                });
            }

            return Ok(response);
        }

        // PATCH /{id}/active - sets IsActive for user
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch("{id}/active")]
        public async Task<ActionResult> SetIsActive(IsActiveRequest request, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Cannot deactivate admins
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(Roles.Admin))
                return BadRequest("Cannot deactivate admin users.");

            // Admin cannot deactivate his/herself
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
                return BadRequest("You cannot deactivate your own account.");

            // No need to change IsActive if it is set to the same value
            if (user.IsActive == request.IsActive)
            {
                return Ok(new
                {
                    Message = $"{user.Email}'s active status is already {user.IsActive}."
                });
            }

            user.IsActive = request.IsActive;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            return Ok(new
            {
                Message = $"{user.Email}'s active status has been set to {user.IsActive}."
            });
        }

        // DELETE /{id} - deletes a user
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            return Ok(new
            {
                Message = $"{user.FirstName} {user.LastName} ({user.Email}) has been deleted."
            });
        }

        // PATCH /api/Users/{id}/role - updates user's role
        [Authorize(Roles = Roles.Admin)]
        [HttpPatch("{id}/role")]
        public async Task<ActionResult<UserResponse>> UpdateUserRole(string id, UpdateUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove current roles, if any
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                    return IdentityValidationProblem(removeResult);
            }

            var addResult = await _userManager.AddToRoleAsync(user, request.Role);

            if (!addResult.Succeeded)
                return IdentityValidationProblem(addResult);

            await _userManager.UpdateSecurityStampAsync(user);

            var updatedRoles = await _userManager.GetRolesAsync(user);

            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
                Role = updatedRoles.FirstOrDefault()
            };

            return Ok(response);
        }

        // PATCH /api/Users/{id}/name - updates first and last name
        [Authorize]
        [HttpPatch("{id}/name")]
        public async Task<ActionResult<UserResponse>> UpdateFirstLastName(string id, UpdateFirstLastNameRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Only current user or admin can change names
            var currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser == null)
                return Unauthorized(); // Defensive. [Authorize] should make this unreachable 💪
            var currentLoggedInUserRoles = await _userManager.GetRolesAsync(currentLoggedInUser);

            if (!currentLoggedInUserRoles.Contains(Roles.Admin)
                && !string.Equals(currentLoggedInUser.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            var response = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
            };

            return Ok(response);
        }

        // POST /api/Users/{id}/change-email - updates user's email
        [Authorize]
        [HttpPost("{id}/change-email")]
        public async Task<ActionResult> UpdateEmail(string id, UpdateEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Only current user or admin can change emails
            var currentLoggedInUser = await _userManager.GetUserAsync(User);
            if (currentLoggedInUser == null) return Unauthorized();
            var currentLoggedInUserRoles = await _userManager.GetRolesAsync(currentLoggedInUser);

            if (!currentLoggedInUserRoles.Contains(Roles.Admin)
                && !string.Equals(currentLoggedInUser.Id, id, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var requestEmail = request.Email.Trim();

            // Check if the requested email is the same as the current user's email
            if (string.Equals(user.Email, requestEmail, StringComparison.OrdinalIgnoreCase))
                return BadRequest("That is already your current email address.");

            // Ensure no other user has the requested email address
            var existingUser = await _userManager.FindByEmailAsync(requestEmail);
            if (existingUser != null)
            {
                return BadRequest("An account with this email already exists.");
            }

            // Send confirmation email
            try
            {
                await _emailService.SendChangeEmailConfirmAsync(
                    requestEmail,
                    user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send confirmation email to {Email}",
                    request.Email);

                return StatusCode(500,
                    "We couldn't send the confirmation email. " +
                    "Please try the 'Resend confirmation email' option.");
            }

            return Ok("Please check your email to confirm your email address.");
        }

        // GET /api/Users/change-email-confirm - confirms the new email and updates email and user name
        [HttpGet("change-email-confirm")]
        public async Task<ActionResult> ConfirmChangeEmail(string newEmail, string userId, string token)
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

            // Change email
            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            // Update user name, also
            result = await _userManager.SetUserNameAsync(user, newEmail);
            if (!result.Succeeded)
                return IdentityValidationProblem(result);

            // Require user to re-login after updating email & user name
            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new
            {
                Message = "Email was successfully confirmed and changed!"
            });
        }
    }
}