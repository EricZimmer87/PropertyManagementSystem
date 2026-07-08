using PropertyManagement.Api.DTOs.Bookings;
using PropertyManagement.Api.Models;
using System.Linq.Expressions;

namespace PropertyManagement.Api.Services.Bookings
{
    public static class BookingProjections
    {
        public static readonly Expression<Func<Booking, BookingResponse>> ToBookingResponse =
            b => new BookingResponse
            {
                BookingId = b.BookingId,

                GuestName =
                    (b.Guest.FirstName ?? "") + " " + (b.Guest.LastName ?? ""),

                UnitNumber = b.Unit.UnitNumber,

                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Occupants = b.Occupants,
                Status = b.Status,
                Notes = b.Notes,
                CardLastFour = b.CardLastFour,

                CreatedOn = b.CreatedOn,
                ModifiedOn = b.ModifiedOn,
                CanceledOn = b.CanceledOn,

                CreatedByUserName =
                    b.CreatedByUser == null
                        ? string.Empty
                        : (string.IsNullOrEmpty(b.CreatedByUser.FirstName) ||
                           string.IsNullOrEmpty(b.CreatedByUser.LastName))
                            ? b.CreatedByUser.Email
                            : b.CreatedByUser.FirstName + " " + b.CreatedByUser.LastName,

                ModifiedByUserName =
                    b.ModifiedByUser == null
                        ? string.Empty
                        : (string.IsNullOrEmpty(b.ModifiedByUser.FirstName) ||
                           string.IsNullOrEmpty(b.ModifiedByUser.LastName))
                            ? b.ModifiedByUser.Email
                            : b.ModifiedByUser.FirstName + " " + b.ModifiedByUser.LastName,

                CanceledByUserName =
                    b.CanceledByUser == null
                        ? string.Empty
                        : (string.IsNullOrEmpty(b.CanceledByUser.FirstName) ||
                           string.IsNullOrEmpty(b.CanceledByUser.LastName))
                            ? b.CanceledByUser.Email
                            : b.CanceledByUser.FirstName + " " + b.CanceledByUser.LastName
            };
    }
}
