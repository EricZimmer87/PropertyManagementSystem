using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Seeding
{
    public class SampleBookings
    {
        public static void Seed(AppDbContext context)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = DateTime.UtcNow;

            var units = context.Units
                .Where(u => u.UnitNumber != null)
                .ToDictionary(u => u.UnitNumber!);

            var guests = context.Guests
                .Where(g => g.NormalizedPhoneNumber != null)
                .ToDictionary(g => g.NormalizedPhoneNumber!);

            context.Bookings.AddRange(
                new Booking
                {
                    GuestId = guests["5550101"].GuestId,
                    UnitId = units["101"].UnitId,
                    StartDate = today.AddDays(-2),
                    EndDate = today.AddDays(2),
                    Occupants = 4,
                    Status = BookingStatus.Booked,
                    Notes = "Family vacation.",
                    CreatedOn = now.AddDays(-20)
                },
                new Booking
                {
                    GuestId = guests["5550102"].GuestId,
                    UnitId = units["102"].UnitId,
                    StartDate = today,
                    EndDate = today.AddDays(1),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    Notes = "Business traveler. Arriving today.",
                    CreatedOn = now.AddDays(-10)
                },
                new Booking
                {
                    GuestId = guests["5550103"].GuestId,
                    UnitId = units["103"].UnitId,
                    StartDate = today.AddDays(-3),
                    EndDate = today,
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Checking out today.",
                    CreatedOn = now.AddDays(-14)
                },
                new Booking
                {
                    GuestId = guests["5550104"].GuestId,
                    UnitId = units["104"].UnitId,
                    StartDate = today.AddDays(1),
                    EndDate = today.AddDays(4),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Returning guest.",
                    CreatedOn = now.AddDays(-7)
                },
                new Booking
                {
                    GuestId = guests["5550105"].GuestId,
                    UnitId = units["105"].UnitId,
                    StartDate = today.AddDays(3),
                    EndDate = today.AddDays(6),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    CreatedOn = now.AddDays(-5)
                },
                new Booking
                {
                    GuestId = guests["5550106"].GuestId,
                    UnitId = units["106"].UnitId,
                    StartDate = today.AddDays(5),
                    EndDate = today.AddDays(8),
                    Occupants = 4,
                    Status = BookingStatus.Booked,
                    Notes = "Requested extra towels.",
                    CreatedOn = now.AddDays(-4)
                },
                new Booking
                {
                    GuestId = guests["5550107"].GuestId,
                    UnitId = units["107"].UnitId,
                    StartDate = today.AddDays(-1),
                    EndDate = today.AddDays(1),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    CreatedOn = now.AddDays(-8)
                },
                new Booking
                {
                    GuestId = guests["5550108"].GuestId,
                    UnitId = units["108"].UnitId,
                    StartDate = today.AddDays(7),
                    EndDate = today.AddDays(10),
                    Occupants = 1,
                    Status = BookingStatus.Canceled,
                    Notes = "Canceled due to schedule change.",
                    CreatedOn = now.AddDays(-12),
                    CanceledOn = now.AddDays(-2)
                },
                new Booking
                {
                    GuestId = guests["5550111"].GuestId,
                    UnitId = units["109"].UnitId,
                    StartDate = today,
                    EndDate = today.AddDays(5),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    Notes = "Traveling nurse.",
                    CreatedOn = now.AddDays(-11)
                },
                new Booking
                {
                    GuestId = guests["5550114"].GuestId,
                    UnitId = units["110"].UnitId,
                    StartDate = today.AddDays(6),
                    EndDate = today.AddDays(9),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Requested accessible room.",
                    CreatedOn = now.AddDays(-6)
                },
                new Booking
                {
                    GuestId = guests["5550113"].GuestId,
                    UnitId = units["115"].UnitId,
                    StartDate = today.AddDays(-1),
                    EndDate = today.AddDays(3),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "King suite. Business stay.",
                    CreatedOn = now.AddDays(-9)
                },
                new Booking
                {
                    GuestId = guests["5550112"].GuestId,
                    UnitId = units["116"].UnitId,
                    StartDate = today.AddDays(10),
                    EndDate = today.AddDays(13),
                    Occupants = 3,
                    Status = BookingStatus.Booked,
                    Notes = "King suite.",
                    CreatedOn = now.AddDays(-3)
                },

                // Camper bookings
                new Booking
                {
                    GuestId = guests["5550109"].GuestId,
                    UnitId = units["C1"].UnitId,
                    StartDate = today.AddDays(-4),
                    EndDate = today.AddDays(3),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Camper site. Extended weekend.",
                    CreatedOn = now.AddDays(-18)
                },
                new Booking
                {
                    GuestId = guests["5550110"].GuestId,
                    UnitId = units["C2"].UnitId,
                    StartDate = today.AddDays(4),
                    EndDate = today.AddDays(8),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Camper site.",
                    CreatedOn = now.AddDays(-6)
                },
                new Booking
                {
                    GuestId = guests["5550115"].GuestId,
                    UnitId = units["C3"].UnitId,
                    StartDate = today.AddDays(-2),
                    EndDate = today.AddDays(4),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    Notes = "Camper site.",
                    CreatedOn = now.AddDays(-13)
                },
                new Booking
                {
                    GuestId = guests["5550117"].GuestId,
                    UnitId = units["C4"].UnitId,
                    StartDate = today.AddDays(2),
                    EndDate = today.AddDays(5),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Fishing trip.",
                    CreatedOn = now.AddDays(-4)
                },
                new Booking
                {
                    GuestId = guests["5550118"].GuestId,
                    UnitId = units["C5"].UnitId,
                    StartDate = today.AddDays(9),
                    EndDate = today.AddDays(12),
                    Occupants = 2,
                    Status = BookingStatus.Canceled,
                    Notes = "Canceled reservation for camper site.",
                    CreatedOn = now.AddDays(-15),
                    CanceledOn = now.AddDays(-5)
                },
                new Booking
                {
                    GuestId = guests["5550120"].GuestId,
                    UnitId = units["C6"].UnitId,
                    StartDate = today.AddDays(-1),
                    EndDate = today.AddDays(6),
                    Occupants = 3,
                    Status = BookingStatus.Booked,
                    Notes = "Camper site. Family stay.",
                    CreatedOn = now.AddDays(-10)
                },
                new Booking
                {
                    GuestId = guests["5550119"].GuestId,
                    UnitId = units["C7"].UnitId,
                    StartDate = today.AddDays(12),
                    EndDate = today.AddDays(15),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    Notes = "Camper site. Work trip.",
                    CreatedOn = now.AddDays(-2)
                },
                // Repeat guest - John Anderson
                new Booking
                {
                    GuestId = guests["5550101"].GuestId,
                    UnitId = units["111"].UnitId,
                    StartDate = today.AddDays(18),
                    EndDate = today.AddDays(21),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    Notes = "Returning guest.",
                    CreatedOn = now.AddDays(-1)
                },

                // Repeat guest - Sarah Lopez
                new Booking
                {
                    GuestId = guests["5550104"].GuestId,
                    UnitId = units["112"].UnitId,
                    StartDate = today.AddDays(25),
                    EndDate = today.AddDays(28),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    CreatedOn = now.AddHours(-12)
                },

                // Future family reservation
                new Booking
                {
                    GuestId = guests["5550124"].GuestId,
                    UnitId = units["113"].UnitId,
                    StartDate = today.AddDays(14),
                    EndDate = today.AddDays(17),
                    Occupants = 4,
                    Status = BookingStatus.Booked,
                    Notes = "Family reunion.",
                    CreatedOn = now.AddDays(-2)
                },

                // Weekend getaway
                new Booking
                {
                    GuestId = guests["5550121"].GuestId,
                    UnitId = units["114"].UnitId,
                    StartDate = today.AddDays(8),
                    EndDate = today.AddDays(10),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    CreatedOn = now.AddDays(-4)
                },

                // Business traveler
                new Booking
                {
                    GuestId = guests["5550122"].GuestId,
                    UnitId = units["115"].UnitId,
                    StartDate = today.AddDays(15),
                    EndDate = today.AddDays(18),
                    Occupants = 1,
                    Status = BookingStatus.Booked,
                    Notes = "Late check-in requested.",
                    CreatedOn = now.AddDays(-3)
                },

                // Last minute reservation
                new Booking
                {
                    GuestId = guests["5550123"].GuestId,
                    UnitId = units["116"].UnitId,
                    StartDate = today.AddDays(1),
                    EndDate = today.AddDays(2),
                    Occupants = 2,
                    Status = BookingStatus.Booked,
                    CreatedOn = now.AddHours(-6)
                });
        }
    }
}
