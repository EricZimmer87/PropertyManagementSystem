using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;

namespace PropertyManagement.Api.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unit> Units => Set<Unit>();

        public DbSet<Guest> Guests => Set<Guest>();

        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<AllowedEmail> AllowedEmails => Set<AllowedEmail>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // UnitNumber must be unique
            builder.Entity<Unit>()
                .HasIndex(x => x.UnitNumber)
                .IsUnique();

            // Guest normalized phone number must be unique
            builder.Entity<Guest>()
            .HasIndex(g => g.NormalizedPhoneNumber)
            .IsUnique();

            builder.Entity<Booking>()
                .HasOne(x => x.CreatedByUser)
                .WithMany(x => x.CreatedBookings)
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(x => x.ModifiedByUser)
                .WithMany(x => x.ModifiedBookings)
                .HasForeignKey(x => x.ModifiedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Booking>()
                .HasOne(x => x.CanceledByUser)
                .WithMany(x => x.CanceledBookings)
                .HasForeignKey(x => x.CanceledByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        // Normalize phone numbers
        private void NormalizeGuestPhoneNumbers()
        {
            foreach (var entry in ChangeTracker.Entries<Guest>()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified))
            {
                entry.Entity.NormalizedPhoneNumber =
                    PhoneNumberHelper.Normalize(entry.Entity.PhoneNumber);
            }
        }

        // Override SaveChanges() and SaveChangesAsync() to automatically normalize phone number of guest
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            NormalizeGuestPhoneNumbers();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override int SaveChanges()
        {
            NormalizeGuestPhoneNumbers();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            NormalizeGuestPhoneNumbers();
            return base.SaveChangesAsync(cancellationToken);
        }

        // Override OnConfiguring to seed database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseSeeding((context, _) =>
                {
                    if (!context.Set<Unit>().Any())
                    {
                        context.Set<Unit>().AddRange(GetSampleUnits());
                        context.SaveChanges();
                    }

                    if (!context.Set<Guest>().Any())
                    {
                        context.Set<Guest>().AddRange(GetSampleGuests());
                        context.SaveChanges();
                    }

                    if (!context.Set<Booking>().Any())
                    {
                        SeedSampleBookings((AppDbContext)context);
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    if (!context.Set<Unit>().Any())
                    {
                        context.Set<Unit>().AddRange(GetSampleUnits());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!context.Set<Guest>().Any())
                    {
                        context.Set<Guest>().AddRange(GetSampleGuests());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!context.Set<Booking>().Any())
                    {
                        SeedSampleBookings((AppDbContext)context);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });

        // Sample Units
        private static IEnumerable<Unit> GetSampleUnits() =>
            [
            new() { UnitNumber = "101", UnitType = "Double Queen" },
            new() { UnitNumber = "102", UnitType = "Double Queen" },
            new() { UnitNumber = "103", UnitType = "Double Queen" },
            new() { UnitNumber = "104", UnitType = "Double Queen" },
            new() { UnitNumber = "105", UnitType = "Double Queen" },
            new() { UnitNumber = "106", UnitType = "Double Queen" },
            new() { UnitNumber = "107", UnitType = "Double Queen" },
            new() { UnitNumber = "108", UnitType = "Double Queen" },
            new() { UnitNumber = "109", UnitType = "Double Queen" },
            new() { UnitNumber = "110", UnitType = "Handicap, Single Queen" },
            new() { UnitNumber = "111", UnitType = "Double Queen" },
            new() { UnitNumber = "112", UnitType = "Double Queen" },
            new() { UnitNumber = "113", UnitType = "Double Queen" },
            new() { UnitNumber = "114", UnitType = "Double Queen" },
            new() { UnitNumber = "115", UnitType = "King Suite" },
            new() { UnitNumber = "116", UnitType = "King Suite" },
            new() { UnitNumber = "C1", UnitType = "Camper" },
            new() { UnitNumber = "C2", UnitType = "Camper" },
            new() { UnitNumber = "C3", UnitType = "Camper" },
            new() { UnitNumber = "C4", UnitType = "Camper" },
            new() { UnitNumber = "C5", UnitType = "Camper" },
            new() { UnitNumber = "C6", UnitType = "Camper" },
            new() { UnitNumber = "C7", UnitType = "Camper" }
            ];

        // Sample Guests
        private static IEnumerable<Guest> GetSampleGuests() =>
            [
                new()
                {
                    FirstName = "John",
                    LastName = "Anderson",
                    PhoneNumber = "555-0101",
                    Email = "john.anderson@example.com",
                    City = "Chicago",
                    State = "IL"
                },
                new()
                {
                    FirstName = "Emily",
                    LastName = "Parker",
                    PhoneNumber = "555-0102",
                    Email = "emily.parker@example.com",
                    Company = "Acme Consulting"
                },
                new()
                {
                    FirstName = "Michael",
                    LastName = "Reed",
                    PhoneNumber = "555-0103",
                    Email = "michael.reed@example.com",
                    Company = "Midwest Construction"
                },
                new()
                {
                    FirstName = "Sarah",
                    LastName = "Lopez",
                    PhoneNumber = "555-0104",
                    Email = "sarah.lopez@example.com",
                    Notes = "Prefers first floor."
                },
                new()
                {
                    FirstName = "David",
                    LastName = "Chen",
                    PhoneNumber = "555-0105",
                    Email = "david.chen@example.com"
                },
                new()
                {
                    FirstName = "Ashley",
                    LastName = "Morgan",
                    PhoneNumber = "555-0106",
                    Email = "ashley.morgan@example.com"
                },
                new()
                {
                    FirstName = "Robert",
                    LastName = "Hill",
                    PhoneNumber = "555-0107",
                    Email = "robert.hill@example.com"
                },
                new()
                {
                    FirstName = "Jennifer",
                    LastName = "Scott",
                    PhoneNumber = "555-0108",
                    Email = "jennifer.scott@example.com",
                    Company = "Midwest Electric"
                },
                new()
                {
                    FirstName = "Brian",
                    LastName = "Wilson",
                    PhoneNumber = "555-0109",
                    Email = "brian.wilson@example.com"
                },
                new()
                {
                    FirstName = "Lisa",
                    LastName = "Thompson",
                    PhoneNumber = "555-0110",
                    Email = "lisa.thompson@example.com"
                },
                new()
                {
                    FirstName = "Kevin",
                    LastName = "Brooks",
                    PhoneNumber = "555-0111",
                    Email = "kevin.brooks@example.com",
                    Company = "Mercy Hospital"
                },
                new()
                {
                    FirstName = "Melissa",
                    LastName = "Green",
                    PhoneNumber = "555-0112",
                    Email = "melissa.green@example.com"
                },
                new()
                {
                    FirstName = "Daniel",
                    LastName = "Foster",
                    PhoneNumber = "555-0113",
                    Email = "daniel.foster@example.com",
                    Company = "State Farm"
                },
                new()
                {
                    FirstName = "Amanda",
                    LastName = "White",
                    PhoneNumber = "555-0114",
                    Email = "amanda.white@example.com"
                },
                new()
                {
                    FirstName = "Charles",
                    LastName = "King",
                    PhoneNumber = "555-0115",
                    Email = "charles.king@example.com"
                },
                new()
                {
                    FirstName = "Nicole",
                    LastName = "Evans",
                    PhoneNumber = "555-0116",
                    Email = "nicole.evans@example.com"
                },
                new()
                {
                    FirstName = "Steven",
                    LastName = "Young",
                    PhoneNumber = "555-0117",
                    Email = "steven.young@example.com"
                },
                new()
                {
                    FirstName = "Rachel",
                    LastName = "Hall",
                    PhoneNumber = "555-0118",
                    Email = "rachel.hall@example.com"
                },
                new()
                {
                    FirstName = "Mark",
                    LastName = "Turner",
                    PhoneNumber = "555-0119",
                    Email = "mark.turner@example.com",
                    Company = "Turner Plumbing"
                },
                new()
                {
                    FirstName = "Olivia",
                    LastName = "Baker",
                    PhoneNumber = "555-0120",
                    Email = "olivia.baker@example.com"
                },
                new()
                {
                    FirstName = "James",
                    LastName = "Collins",
                    PhoneNumber = "555-0121",
                    Email = "james.collins@example.com"
                },
                new()
                {
                    FirstName = "Karen",
                    LastName = "Price",
                    PhoneNumber = "555-0122",
                    Email = "karen.price@example.com"
                },
                new()
                {
                    FirstName = "Tyler",
                    LastName = "Murphy",
                    PhoneNumber = "555-0123",
                    Email = "tyler.murphy@example.com"
                },
                new()
                {
                    FirstName = "Heather",
                    LastName = "Ward",
                    PhoneNumber = "555-0124",
                    Email = "heather.ward@example.com"
                }
            ];

        // Sample Bookings
        private static void SeedSampleBookings(AppDbContext context)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var now = DateTime.UtcNow;

            var units = context.Units.ToDictionary(x => x.UnitNumber);
            var guests = context.Guests
                .ToDictionary(x => x.NormalizedPhoneNumber);

            context.Bookings.AddRange(
                new Booking
                {
                    GuestId = guests["5550101"].GuestId,
                    UnitId = units["101"].UnitId,
                    StartDate = today.AddDays(-2),
                    EndDate = today.AddDays(2),
                    Occupants = 4,
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
                    CreatedOn = now.AddDays(-5)
                },
                new Booking
                {
                    GuestId = guests["5550106"].GuestId,
                    UnitId = units["106"].UnitId,
                    StartDate = today.AddDays(5),
                    EndDate = today.AddDays(8),
                    Occupants = 4,
                    Status = "Booked",
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
                    Status = "Booked",
                    CreatedOn = now.AddDays(-8)
                },
                new Booking
                {
                    GuestId = guests["5550108"].GuestId,
                    UnitId = units["108"].UnitId,
                    StartDate = today.AddDays(7),
                    EndDate = today.AddDays(10),
                    Occupants = 1,
                    Status = "Canceled",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Canceled",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
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
                    Status = "Booked",
                    CreatedOn = now.AddHours(-6)
                });
            }


    }
}
