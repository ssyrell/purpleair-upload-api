using System;
namespace SteveSyrell.PurpleAirUploadApi
{
    public struct AveragerThresholds
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public DateTimeOffset TenMinutes { get; set; }
        public DateTimeOffset? OneHour { get; set; }
        public DateTimeOffset? OneDay { get; set; }
        public DateTimeOffset? OneMonth { get; set; }
    }
}