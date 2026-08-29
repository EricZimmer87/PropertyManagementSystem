using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Common;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.DTOs.Units;
using PropertyManagement.Api.DTOs.Shared;

namespace PropertyManagement.Api.Services.Units
{
    public class UnitService : IUnitService
    {
        private readonly AppDbContext _context;

        public UnitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<UnitResponse>> GetAllUnitsAsync(QueryFilter filter, CancellationToken cancellationToken = default)
        {
            var pageNumber = Math.Max(1, filter.PageNumber);
            var pageSize = Math.Clamp(filter.PageSize, 1, 50);

            var query = _context.Units
                .AsNoTracking()
                .AsQueryable();

            // Apply search filter
            query = query.ApplySearch(filter.Search);

            // Count total records after filter, before pagination
            var totalRecords = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = query.ApplySort(string.IsNullOrWhiteSpace(filter.SortBy)
                ? "UnitNumber"
                : filter.SortBy);

            // Apply pagination and project to DTO
            var units = await query
                .ApplyPagination(pageNumber, pageSize)
                .Select(u => new UnitResponse
                {
                    UnitId = u.UnitId,
                    UnitNumber = u.UnitNumber,
                    UnitType = u.UnitType,
                    Notes = u.Notes ?? ""
                })
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new PagedResponse<UnitResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1,
                Items = units
            };
        }
    }
}
