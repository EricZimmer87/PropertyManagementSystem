using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Guests;
using PropertyManagement.Api.DTOs.Shared;

namespace PropertyManagement.Api.Services.Guests
{
    public class GuestService : IGuestService
    {
        private readonly AppDbContext _context;

        public GuestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<GuestResponse>> GetAllGuestsAsync(QueryFilter filter, CancellationToken cancellationToken = default)
        {
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, 50);

            var query = _context.Guests
                .AsNoTracking()
                .AsQueryable();

            // Apply search filter
            query = query.ApplySearch(filter.Search);

            // Count total records after filter, before pagination
            var totalRecords = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = query.ApplySort(string.IsNullOrWhiteSpace(filter.SortBy)
                ? "LastName"
                : filter.SortBy);

            // Apply pagination and project to DTO
            var guests = await query
                .ApplyPagination(pageNumber, pageSize)
                .Select(g => new GuestResponse
                {
                    GuestId = g.GuestId,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    PhoneNumber = g.PhoneNumber,
                    NormalizedPhoneNumber = g.NormalizedPhoneNumber,
                    Address = g.Address,
                    City = g.City,
                    State = g.State,
                    ZipCode = g.ZipCode,
                    Email = g.Email,
                    Company = g.Company,
                    Notes = g.Notes
                })
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PagedResponse<GuestResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = guests
            };
        }
    }
}
