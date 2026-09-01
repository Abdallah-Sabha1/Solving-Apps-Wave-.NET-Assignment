using AppsWave.Models;
using AppsWave.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppsWave.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController(BookingService bookingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Booking>> Create(Booking booking, CancellationToken cancellationToken)
    {
        try
        {
            var created = await bookingService.CreateAsync(booking, cancellationToken);
            return CreatedAtAction(nameof(GetByResource), new { resourceId = created.ResourceId }, created);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    [HttpGet("resource/{resourceId}")]
    public async Task<ActionResult<IReadOnlyList<Booking>>> GetByResource(
        string resourceId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetByResourceAsync(resourceId, from, to, cancellationToken);
        return Ok(bookings);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        return await bookingService.CancelAsync(id, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpGet("resource/{resourceId}/availability")]
    public async Task<ActionResult<IReadOnlyList<AvailabilitySlot>>> Availability(
        string resourceId, DateTime from, DateTime to, int durationHours, CancellationToken cancellationToken)
    {
        try
        {
            var slots = await bookingService.GetAvailabilityAsync(
                resourceId, from, to, durationHours, cancellationToken);
            return Ok(slots);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}
