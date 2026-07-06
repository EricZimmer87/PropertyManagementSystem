using PropertyManagement.Api.Common;
using PropertyManagement.Api.DTOs.Guests;
using PropertyManagement.Api.DTOs.Shared;

namespace PropertyManagement.Api.Services.Guests
{
    public interface IGuestService
    {
        Task<PagedResponse<GuestResponse>> GetAllGuestsAsync(QueryFilter filter, CancellationToken cancellationToken = default);
    }
}
