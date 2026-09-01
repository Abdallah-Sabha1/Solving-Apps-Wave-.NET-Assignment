using AppsWave.Models;
using Microsoft.EntityFrameworkCore;

namespace AppsWave.Data;

public class AppsWaveDbContext(DbContextOptions<AppsWaveDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();
}
