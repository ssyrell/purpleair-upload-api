using System;

namespace SteveSyrell.PurpleAirUploadApi
{
    public class Utilities
    {
        public string GenerateRowKey()
        {
            return string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
        }

        public DateTimeOffset RoundToNearestTenMinutes(DateTimeOffset value)
        {
            if (value.Minute < 5)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 0, 0, value.Offset);
            }

            if (value.Minute < 15)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 10, 0, value.Offset);
            }

            if (value.Minute < 25)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 20, 0, value.Offset);
            }

            if (value.Minute < 35)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 30, 0, value.Offset);
            }

            if (value.Minute < 45)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 40, 0, value.Offset);
            }

            if (value.Minute < 55)
            {
                return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, 50, 0, value.Offset);
            }

            var minutesToAdd = 60 - value.Minute;
            return new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Offset).AddMinutes(minutesToAdd);
        }

        public AveragerThresholds CalculateAveragerThresholds(DateTimeOffset now)
        {
            var roundedNow = this.RoundToNearestTenMinutes(now);
            var thresholds = new AveragerThresholds
            {
                Start = new DateTimeOffset(roundedNow.Year, roundedNow.Month, roundedNow.Day, roundedNow.Hour, roundedNow.Minute, 0, roundedNow.Offset)
            };
            thresholds.TenMinutes = thresholds.Start.AddMinutes(-10);
            thresholds.End = thresholds.TenMinutes;

            if (thresholds.Start.Minute == 0)
            {
                thresholds.OneHour = thresholds.Start.AddHours(-1);
                thresholds.End = thresholds.OneHour.Value;

                if (thresholds.Start.Hour == 0)
                {
                    thresholds.OneDay = thresholds.Start.AddDays(-1);
                    thresholds.End = thresholds.OneDay.Value;

                    if (thresholds.Start.Day == 1)
                    {
                        thresholds.OneMonth = thresholds.Start.AddMonths(-1);
                        thresholds.End = thresholds.OneMonth.Value;
                    }
                }
            }

            return thresholds;
        }
    }
}