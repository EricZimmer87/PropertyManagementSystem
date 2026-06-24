using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Data;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Seeding
{
    public static class DbSeeding
    {
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSeeding((context, _) =>
                {
                    // Seed Units
                    if (!context.Set<Unit>().Any())
                    {
                        context.Set<Unit>().AddRange(SampleUnits.Get());
                        context.SaveChanges();
                    }

                    // Seed Guests
                    if (!context.Set<Guest>().Any())
                    {
                        context.Set<Guest>().AddRange(SampleGuests.Get());
                        context.SaveChanges();
                    }

                    // Seed Bookings
                    if (!context.Set<Booking>().Any())
                    {
                        SampleBookings.Seed((AppDbContext)context);
                        context.SaveChanges();
                    }

                    // Seed first allowed email to be used as admin user
                    if (!context.Set<AllowedEmail>().Any())
                    {
                        context.Add(new AllowedEmail { Email = "ericzimmer87@fastmail.com", CreatedAt = DateTime.UtcNow });
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    // Seed Units
                    if (!await context.Set<Unit>().AnyAsync(cancellationToken))
                    {
                        context.Set<Unit>().AddRange(SampleUnits.Get());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed Guests
                    if (!await context.Set<Guest>().AnyAsync(cancellationToken))
                    {
                        context.Set<Guest>().AddRange(SampleGuests.Get());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed Bookings
                    if (!await context.Set<Booking>().AnyAsync(cancellationToken))
                    {
                        SampleBookings.Seed((AppDbContext)context);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed first allowed email to be used as admin user
                    if (!await context.Set<AllowedEmail>().AnyAsync(cancellationToken))
                    {
                        context.Add(new AllowedEmail { Email = "ericzimmer87@fastmail.com", CreatedAt = DateTime.UtcNow });
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });
        }
    }
}
