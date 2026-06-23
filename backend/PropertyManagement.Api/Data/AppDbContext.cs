using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Models;

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
    }
}
