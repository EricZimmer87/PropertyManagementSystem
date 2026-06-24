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
    }
}
