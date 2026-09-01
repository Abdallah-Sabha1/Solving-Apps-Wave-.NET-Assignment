using AppsWave.Enums;
using AppsWave.Models;

namespace AppsWave.Services;

public class BookingOverlapChecker
{
    public bool Overlapping(Booking currentBook, Booking requestedBook)
    {
        if (currentBook.ResourceId != requestedBook.ResourceId)
        {
            return false;
        }

        if (currentBook.Status == BookingStatus.Cancelled)
        {
            return false;
        }

        return requestedBook.StartDateTime < currentBook.EndDateTime
            && currentBook.StartDateTime < requestedBook.EndDateTime;
    }
}
