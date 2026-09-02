using AppsWave.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppsWave.Data;

public class AppsWaveDbContext(DbContextOptions<AppsWaveDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            value => value.ToUniversalTime(),
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

        modelBuilder.Entity<Booking>().Property(booking => booking.CreatedAt).HasConversion(utcConverter);
        modelBuilder.Entity<Booking>().Property(booking => booking.StartDateTime).HasConversion(utcConverter);
        modelBuilder.Entity<Booking>().Property(booking => booking.EndDateTime).HasConversion(utcConverter);
    }
}
