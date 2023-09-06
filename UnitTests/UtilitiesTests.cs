using System;
using Xunit;
using SteveSyrell.PurpleAirUploadApi;

namespace Functions.UnitTests;

public class UtilitiesTests
{
    private Utilities utilities;

    public UtilitiesTests()
    {
        this.utilities = new Utilities();
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToZero()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 0, 1, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 1, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 4, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToTen()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 10, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 5, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 10, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 9, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 10, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 11, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 10, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 14, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToTwenty()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 20, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 15, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 20, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 19, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 20, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 21, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 20, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 24, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToThirty()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 30, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 25, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 30, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 29, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 30, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 31, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 30, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 34, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToForty()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 40, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 35, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 40, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 39, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 40, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 41, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 40, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 44, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToFifty()
    {
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 50, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 45, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 50, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 49, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 50, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 51, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 0, 50, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 54, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void RoundToNearestTenMinutes_RoundToNextHour()
    {
        // Expect roll over to 1am
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 1, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 55, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 1, 1, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 0, 59, 0, TimeSpan.Zero)));

        // Expect roll over to midnight the next day
        Assert.Equal(
            new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 23, 55, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 1, 23, 59, 0, TimeSpan.Zero)));

        // Expect roll over to midnight of the first day of the next month
        Assert.Equal(
            new DateTimeOffset(2023, 2, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 31, 23, 55, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2023, 2, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 1, 31, 23, 59, 0, TimeSpan.Zero)));

        // Expect roll over to midnight of the first day of the next year
        Assert.Equal(
            new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 12, 31, 23, 55, 0, TimeSpan.Zero)));
        Assert.Equal(
            new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
            this.utilities.RoundToNearestTenMinutes(new DateTimeOffset(2023, 12, 31, 23, 59, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void CalculateAveragerThresholds_TenMinuteWindow()
    {
        for (int minute = 10; minute < 60; minute += 10)
        {
            var start = new DateTimeOffset(2023, 1, 1, 1, minute, 0, TimeSpan.Zero);
            var thresholds = this.utilities.CalculateAveragerThresholds(start);
            Assert.Equal(start, thresholds.Start);
            Assert.Equal(start.AddMinutes(-10), thresholds.End);
            Assert.Equal(thresholds.End, thresholds.TenMinutes);
            Assert.False(thresholds.OneHour.HasValue);
            Assert.False(thresholds.OneDay.HasValue);
            Assert.False(thresholds.OneMonth.HasValue);
        }
    }

    [Fact]
    public void CalculateAveragerThresholds_OneHourWindow()
    {
        var start = new DateTimeOffset(2023, 1, 1, 1, 0, 0, TimeSpan.Zero);
        var thresholds = this.utilities.CalculateAveragerThresholds(start);
        Assert.Equal(start, thresholds.Start);
        Assert.Equal(thresholds.OneHour, thresholds.End);
        Assert.Equal(start.AddMinutes(-10), thresholds.TenMinutes);
        Assert.Equal(start.AddHours(-1), thresholds.OneHour);
        Assert.False(thresholds.OneDay.HasValue);
        Assert.False(thresholds.OneMonth.HasValue);
    }

    [Fact]
    public void CalculateAveragerThresholds_OneDayWindow()
    {
        var start = new DateTimeOffset(2023, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var thresholds = this.utilities.CalculateAveragerThresholds(start);
        Assert.Equal(start, thresholds.Start);
        Assert.Equal(thresholds.OneDay, thresholds.End);
        Assert.Equal(start.AddMinutes(-10), thresholds.TenMinutes);
        Assert.Equal(start.AddHours(-1), thresholds.OneHour);
        Assert.Equal(start.AddDays(-1), thresholds.OneDay);
        Assert.False(thresholds.OneMonth.HasValue);
    }

    [Fact]
    public void CalculateAveragerThresholds_OneMonthWindow()
    {
        var start = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var thresholds = this.utilities.CalculateAveragerThresholds(start);
        Assert.Equal(start, thresholds.Start);
        Assert.Equal(thresholds.OneMonth, thresholds.End);
        Assert.Equal(start.AddMinutes(-10), thresholds.TenMinutes);
        Assert.Equal(start.AddHours(-1), thresholds.OneHour);
        Assert.Equal(start.AddDays(-1), thresholds.OneDay);
        Assert.Equal(start.AddMonths(-1), thresholds.OneMonth);
    }
}