using AppsWave.Enums;
using AppsWave.Models;
using AppsWave.Services;
namespace AppsWave.Tests
{
    public class BookingOverlapCheckerTests
    {
        [Fact]
        public void ReturnsTrue_WhenBookingsOverlap()
        {
            //Phase 1  : Arrange
            Booking currentBooking = new Booking();
            currentBooking.ResourceId = "room-1";
            currentBooking.UserId = "1";
            currentBooking.StartDateTime =new DateTime(2026,9,3,10,0,0,DateTimeKind.Utc);
            currentBooking.EndDateTime = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            Booking requestedBooking = new Booking();
            requestedBooking.ResourceId = "room-1";
            requestedBooking.UserId = "2";
            requestedBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 30, 0, DateTimeKind.Utc);
            requestedBooking.EndDateTime = new DateTime(2026, 9, 3, 11, 30, 0, DateTimeKind.Utc);

            //Phase 2  : Act

            BookingOverlapCheckerService bookingOverlapChecker = new BookingOverlapCheckerService();
            bool actualResult = bookingOverlapChecker.Overlapping(currentBooking, requestedBooking);

            //Phase3 : Assert(Check Expected Value which is (True) with the Actual Value from the Act Phase ! ) 
            Assert.True(actualResult);
        }
        [Fact]
        public void ReturnsFalse_WhenBookingsAreAdjacent()
        {
            //Phase 1  : Arrange
            Booking currentBooking = new Booking();
            currentBooking.ResourceId = "room-1";
            currentBooking.UserId = "1";
            currentBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc);
            currentBooking.EndDateTime = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            Booking requestedBooking = new Booking();
            requestedBooking.ResourceId = "room-1";
            requestedBooking.UserId = "2";
            requestedBooking.StartDateTime = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            requestedBooking.EndDateTime = new DateTime(2026, 9, 3, 13, 0, 0, DateTimeKind.Utc);

            //Phase 2  : Act

            BookingOverlapCheckerService bookingOverlapChecker = new BookingOverlapCheckerService();
            bool actualResult = bookingOverlapChecker.Overlapping(currentBooking, requestedBooking);

            //Phase3 : Assert(Check Expected Value which is (False) with the Actual Value from the Act Phase ! ) 
            Assert.False(actualResult);
        }
        [Fact]
        public void ReturnsFalse_WhenCurrentBookingIsCancelled()
        {
            //Phase 1 : Arrange 
            Booking currentBooking = new Booking();
            currentBooking.ResourceId = "room-1";
            currentBooking.UserId = "1";
            currentBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc);
            currentBooking.EndDateTime = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            currentBooking.Status = BookingStatus.Cancelled;
            Booking requestedBooking = new Booking();
            requestedBooking.ResourceId = "room-1";
            requestedBooking.UserId = "2";
            requestedBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 30, 0, DateTimeKind.Utc);
            requestedBooking.EndDateTime = new DateTime(2026, 9, 3, 11, 30, 0, DateTimeKind.Utc);
            requestedBooking.Status = BookingStatus.Active;
            //Phase2
            BookingOverlapCheckerService bookingOverlapChecker = new BookingOverlapCheckerService();
            bool actualResult = bookingOverlapChecker.Overlapping(currentBooking, requestedBooking);
            //Phase3 : Assert(Check Expected Value which is (False) with the Actual Value from the Act Phase ! ) 
            Assert.False(actualResult);
        }
        [Fact]
        public void ReturnsFalse_WhenResourcesAreDifferent()
        {
            //Phase 1 : Arrange 
            Booking currentBooking = new Booking();
            currentBooking.ResourceId = "room-1";
            currentBooking.UserId = "1";
            currentBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc);
            currentBooking.EndDateTime = new DateTime(2026, 9, 3, 12, 0, 0, DateTimeKind.Utc);
            currentBooking.Status = BookingStatus.Cancelled;
            Booking requestedBooking = new Booking();
            requestedBooking.ResourceId = "room-2";
            requestedBooking.UserId = "2";
            requestedBooking.StartDateTime = new DateTime(2026, 9, 3, 10, 30, 0, DateTimeKind.Utc);
            requestedBooking.EndDateTime = new DateTime(2026, 9, 3, 11, 30, 0, DateTimeKind.Utc);
            requestedBooking.Status = BookingStatus.Active;
            //Phase2
            BookingOverlapCheckerService bookingOverlapChecker = new BookingOverlapCheckerService();
            bool actualResult = bookingOverlapChecker.Overlapping(currentBooking, requestedBooking);
            //Phase3 : Assert(Check Expected Value which is (False) with the Actual Value from the Act Phase ! ) 
            Assert.False(actualResult);
        }
    }
}
