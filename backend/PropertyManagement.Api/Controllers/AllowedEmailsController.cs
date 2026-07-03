using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.AllowedEmails;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services.Email;

namespace PropertyManagement.Api.Controllers
{
    [Route("api/allowed-emails")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AllowedEmailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AllowedEmailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/allowed-emails/{id} - get allowed email by id
        [HttpGet("{id}")]
        public async Task<ActionResult<AllowedEmailResponse>> GetAllowedEmailById(long id)
        {
            var email = await _context.AllowedEmails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.AllowedEmailId == id);

            if (email == null)
                return NotFound();

            var response = new AllowedEmailResponse
            {
                AllowedEmailId = email.AllowedEmailId,
                Email = email.Email,
                CreatedAt = email.CreatedAt
            };

            return Ok(response);
        }

        // TODO Add pagination
        // GET /api/allowed-emails - get all allowed emails
        [HttpGet]
        public async Task<ActionResult<List<AllowedEmailResponse>>> GetAllowedEmails()
        {
            var emails = await _context.AllowedEmails
                .AsNoTracking()
                .Select(e => new AllowedEmailResponse
                {
                    AllowedEmailId = e.AllowedEmailId,
                    Email = e.Email,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(emails);
        }

        // POST /api/allowed-emails/add - create a new allowed email
        [HttpPost]
        public async Task<ActionResult<AllowedEmailResponse>> AddAllowedEmail(AddAllowedEmailRequest request)
        {
            var email = request.Email.Trim();

            if (!EmailHelper.IsValidEmail(email))
                return BadRequest("Please enter a valid email address.");

            var normalizedEmail = EmailHelper.Normalize(email);
            if (await _context.AllowedEmails.AnyAsync(
                e => e.NormalizedEmail == normalizedEmail))
            {
                return BadRequest("That email already exists.");
            }

            var newEmail = new AllowedEmail
            {
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            _context.AllowedEmails.Add(newEmail);
            await _context.SaveChangesAsync();

            var response = new AllowedEmailResponse
            {
                AllowedEmailId = newEmail.AllowedEmailId,
                Email = newEmail.Email,
                CreatedAt = newEmail.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetAllowedEmailById),
                new { id = response.AllowedEmailId },
                response);
        }

        // PATCH /api/allowed-emails/{id} - update allowed email
        [HttpPatch("{id}")]
        public async Task<ActionResult<AllowedEmailResponse>> UpdateAllowedEmail(UpdateAllowedEmailRequest request, long id)
        {
            var email = await _context.AllowedEmails
                .FirstOrDefaultAsync(e => e.AllowedEmailId == id);
            if (email == null)
                return NotFound();

            var requestEmail = request.Email.Trim();
            if (!EmailHelper.IsValidEmail(requestEmail))
                return BadRequest("Please enter a valid email address.");

            var normalizedEmail = EmailHelper.Normalize(requestEmail);

            if (await _context.AllowedEmails.AnyAsync(e =>
                e.NormalizedEmail == normalizedEmail &&
                e.AllowedEmailId != id))
            {
                return BadRequest("That email already exists.");
            }

            email.Email = requestEmail;

            await _context.SaveChangesAsync();

            return Ok(new AllowedEmailResponse
            {
                AllowedEmailId = email.AllowedEmailId,
                Email = email.Email,
                CreatedAt = email.CreatedAt
            });
        }

        // DELETE /api/allowed-emails/{id} - delete allowed email
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAllowedEmail(long id)
        {
            var email = await _context.AllowedEmails
                .FirstOrDefaultAsync(e => e.AllowedEmailId == id);

            if (email == null)
                return NotFound();

            _context.AllowedEmails.Remove(email);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
