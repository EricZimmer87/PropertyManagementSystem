using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.AllowedEmails;
using PropertyManagement.Api.DTOs.AppUsers;
using PropertyManagement.Api.DTOs.Shared;
using PropertyManagement.Api.DTOs.Users;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services.Email;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _context;

        public UsersController(UserManager<AppUser> userManager,
            IEmailService emailService,
            ILogger<AuthController> logger,
            AppDbContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _context = context;
        }

        // GET /api/users - gets all users
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            // pageNumber and pageSize must be greater than 0
            if (pageNumber <= 0)
                return BadRequest($"{nameof(pageNumber)} must be greater than 0.");
            if (pageSize <= 0)
                return BadRequest($"{nameof(pageSize)} must be greater than 0.");

            // Users can have only one role, and default is to set to "User" when registering.
            // Users will have one and only one role.
            // If that changes, so must this query.
            var totalUsers = await _context.Users.CountAsync();

            int skip = (pageNumber - 1) * pageSize;

            var userResponses = await (
                from u in _context.Users
                .AsNoTracking()
                join ur in _context.UserRoles on u.Id equals ur.UserId
                join r in _context.Roles on ur.RoleId equals r.Id
                orderby
                    (r.Name == Roles.Admin ? 0 : 1),  // Admin first
                    u.IsActive descending,
                    u.LastName,
                    u.FirstName
                select new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? "",
                    IsActive = u.IsActive,
                    Role = r.Name
                }
            )
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var response = new PagedResponse<UserResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalUsers,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = userResponses
            };

            return Ok(response);
        }

        // PATCH /api/users/{id}/active - sets IsActive for user
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

        // DELETE /api/users/{id} - deletes a user
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Admin cannot delete his/herself
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (currentUser.Id == user.Id)
                return BadRequest("You cannot delete your own account.");

            // Deleting a user also removes their email from AllowedEmails so they
            // cannot recreate their account by signing in again, with it set to active by default.
            var normalizedEmail = _userManager.NormalizeEmail(user.Email);

            var allowedEmail = await _context.AllowedEmails
                .SingleOrDefaultAsync(a => a.NormalizedEmail == normalizedEmail);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                    return IdentityValidationProblem(result);

                if (allowedEmail != null)
                {
                    _context.AllowedEmails.Remove(allowedEmail);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    Message = $"{user.FirstName} {user.LastName} ({user.Email}) has been deleted."
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // PATCH /api/users/{id}/role - updates user's role
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

        // PATCH /api/users/{id}/name - updates first and last name
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

        // POST /api/users/{id}/change-email - updates user's email
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

        // GET /api/users/change-email-confirm - confirms the new email and updates email and user name
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