using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.AppUsers;
using PropertyManagement.Api.DTOs.Users;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class UsersController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET /users - gets all users
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

        // PATCH /api/Users/update-role/{id}
        [HttpPatch("update-role/{id}")]
        public async Task<ActionResult<UserResponse>> UpdateUserRole(string id, UpdateUserRoleRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

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
    }
}