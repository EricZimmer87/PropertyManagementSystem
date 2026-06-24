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
                    if (!context.Set<Unit>().Any())
                    {
                        context.Set<Unit>().AddRange(SampleUnits.Get());
                        context.SaveChanges();
                    }

                    if (!context.Set<Guest>().Any())
                    {
                        context.Set<Guest>().AddRange(SampleGuests.Get());
                        context.SaveChanges();
                    }

                    if (!context.Set<Booking>().Any())
                    {
                        SampleBookings.Seed((AppDbContext)context);
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    if (!await context.Set<Unit>().AnyAsync(cancellationToken))
                    {
                        context.Set<Unit>().AddRange(SampleUnits.Get());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!await context.Set<Guest>().AnyAsync(cancellationToken))
                    {
                        context.Set<Guest>().AddRange(SampleGuests.Get());
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    if (!await context.Set<Booking>().AnyAsync(cancellationToken))
                    {
                        SampleBookings.Seed((AppDbContext)context);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });
        }
    }
}
