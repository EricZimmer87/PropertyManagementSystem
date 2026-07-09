using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.Api.Models;
using PropertyManagement.Api.Services;
using PropertyManagement.Api.Services.Email;

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

            // Allowed emails must be unique
            builder.Entity<AllowedEmail>()
                .HasIndex(x => x.NormalizedEmail)
                .IsUnique();

            // UnitNumber must be unique
            builder.Entity<Unit>()
                .HasIndex(x => x.UnitNumber)
                .IsUnique();

            // Guest normalized phone number must be unique
            builder.Entity<Guest>()
                .HasIndex(g => g.NormalizedPhoneNumber)
                .IsUnique(true);

            // Store BookingStatus as string
            builder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            // Explicitly configure booking audit-user relationships without cascade delete.
            // Users cannot be deleted while referenced by bookings to keep historical records accurate.
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

            // Cannot delete a guest if they have bookings to maintain historical records.
            builder.Entity<Booking>()
                .HasOne(x => x.Guest)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.GuestId)
                .OnDelete(DeleteBehavior.NoAction);
        }

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

        private void NormalizeAllowedEmails()
        {
            foreach (var entry in ChangeTracker.Entries<AllowedEmail>()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified))
            {
                entry.Entity.NormalizedEmail =
                    EmailHelper.Normalize(entry.Entity.Email);
            }
        }

        // Override SaveChanges() and SaveChangesAsync() to automatically normalize phone number of guest
        // and allowed emails
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            NormalizeGuestPhoneNumbers();
            NormalizeAllowedEmails();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override int SaveChanges()
        {
            NormalizeGuestPhoneNumbers();
            NormalizeAllowedEmails();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            NormalizeGuestPhoneNumbers();
            NormalizeAllowedEmails();
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
