namespace AppsWave.Models;
public class Booking
{
    public int Id { get; set; }
    public string ResourceId { get; set; } = "";
    public string UserId { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; } = null;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;

}
