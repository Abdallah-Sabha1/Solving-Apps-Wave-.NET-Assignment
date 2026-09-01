using System;
using AppsWave.Enums;
using AppsWave.Models;
using AppsWave.Services;
using Xunit;

namespace AppsWave.Tests;

public class BookingOverlapCheckerTests
{
    private readonly BookingOverlapChecker checker = new();

    [Fact]
    public void Overlapping_periods_return_true()
    {
        var current = Booking("10:00", "12:00");
        var requested = Booking("10:30", "11:30");

        Assert.True(checker.Overlapping(current, requested));
    }

    [Fact]
    public void Adjacent_periods_do_not_overlap()
    {
        var current = Booking("10:00", "12:00");
        var requested = Booking("12:00", "13:00");

        Assert.False(checker.Overlapping(current, requested));
    }

    [Fact]
    public void Different_resources_do_not_overlap()
    {
        var current = Booking("10:00", "12:00");
        var requested = Booking("10:30", "11:30");
        requested.ResourceId = "room-2";

        Assert.False(checker.Overlapping(current, requested));
    }

    [Fact]
    public void Cancelled_bookings_do_not_overlap()
    {
        var current = Booking("10:00", "12:00");
        current.Status = BookingStatus.Cancelled;
        var requested = Booking("10:30", "11:30");

        Assert.False(checker.Overlapping(current, requested));
    }

    private static Booking Booking(string start, string end) => new()
    {
        ResourceId = "room-1",
        UserId = "user-1",
        StartDateTime = DateTime.Parse($"2026-09-01 {start}Z"),
        EndDateTime = DateTime.Parse($"2026-09-01 {end}Z")
    };
}
