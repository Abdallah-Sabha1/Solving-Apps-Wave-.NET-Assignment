using AppsWave.Data;
using AppsWave.Enums;
using AppsWave.Models;
using Microsoft.EntityFrameworkCore;

namespace AppsWave.Services;

public class BookingService(AppsWaveDbContext db, BookingOverlapChecker overlapChecker)
{
    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken)
    {
        Validate(booking);

        var activeBookings = await db.Bookings
            .Where(x => x.ResourceId == booking.ResourceId && x.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

        if (activeBookings.Any(existing => overlapChecker.Overlapping(existing, booking)))
        {
            throw new InvalidOperationException("The resource is already booked for part of this time period.");
        }

        booking.Id = 0;
        booking.Status = BookingStatus.Active;
        booking.CancelledAt = null;
        booking.CreatedAt = DateTime.UtcNow;
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken);
        return booking;
    }

    public async Task<IReadOnlyList<Booking>> GetByResourceAsync(string resourceId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var query = db.Bookings.AsNoTracking()
            .Where(x => x.ResourceId == resourceId);

        if (from.HasValue)
        {
            query = query.Where(x => x.EndDateTime > from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.StartDateTime < to.Value);
        }

        return await query.OrderBy(x => x.StartDateTime).ToListAsync(cancellationToken);
    }

    public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
    {
        var booking = await db.Bookings.FindAsync([id], cancellationToken);
        if (booking is null || booking.Status == BookingStatus.Cancelled)
        {
            return false;
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AvailabilitySlot>> GetAvailabilityAsync(
        string resourceId, DateTime from, DateTime to, int durationHours, CancellationToken cancellationToken)
    {
        if (durationHours <= 0 || from >= to)
        {
            throw new ArgumentException("The date range and duration must be valid.");
        }

        var activeBookings = await db.Bookings.AsNoTracking()
            .Where(x => x.ResourceId == resourceId && x.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

        var slots = new List<AvailabilitySlot>();
        for (var start = from; start.AddHours(durationHours) <= to; start = start.AddHours(1))
        {
            var candidate = new Booking
            {
                ResourceId = resourceId,
                StartDateTime = start,
                EndDateTime = start.AddHours(durationHours)
            };

            if (activeBookings.All(existing => !overlapChecker.Overlapping(existing, candidate)))
            {
                slots.Add(new AvailabilitySlot(candidate.StartDateTime, candidate.EndDateTime));
            }
        }

        return slots;
    }

    private static void Validate(Booking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.ResourceId))
        {
            throw new ArgumentException("ResourceId is required.");
        }

        if (string.IsNullOrWhiteSpace(booking.UserId))
        {
            throw new ArgumentException("UserId is required.");
        }

        if (booking.StartDateTime.Kind != DateTimeKind.Utc || booking.EndDateTime.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("StartDateTime and EndDateTime must be UTC.");
        }

        if (booking.StartDateTime >= booking.EndDateTime)
        {
            throw new ArgumentException("StartDateTime must be before EndDateTime.");
        }
    }
}
