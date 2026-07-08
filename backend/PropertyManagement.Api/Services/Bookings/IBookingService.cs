using PropertyManagement.Api.Common;
using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.DTOs.Shared;

namespace PropertyManagement.Api.Services.Bookings
{
    public interface IBookingService
    {
        Task<PagedResponse<BookingResponse>> GetAllBookingsAsync(
            QueryFilter filter,
            CancellationToken cancellationToken = default);
    }
}
