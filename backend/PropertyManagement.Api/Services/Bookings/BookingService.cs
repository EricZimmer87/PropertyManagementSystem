using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.DTOs.Shared;
using QueryFilter = PropertyManagement.Api.Common.QueryFilter;

namespace PropertyManagement.Api.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<BookingResponse>> GetAllBookingsAsync(
            QueryFilter filter,
            CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Max(1, filter.PageSize);

            var query = _context.Bookings
                .AsNoTracking()
                .AsQueryable();

            // Apply search filter
            query = query.ApplySearch(filter.Search);

            // Count total records after filter, before pagination
            var totalRecords = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = query.ApplySort(string.IsNullOrWhiteSpace(filter.SortBy)
                ? "Unit.UnitNumber"
                : filter.SortBy);

            // Apply pagination and project to DTO
            var bookings = await query
                .ApplyPagination(pageNumber, pageSize)
                .Select(BookingProjections.ToBookingResponse)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PagedResponse<BookingResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = bookings
            };
        }
    }
}
