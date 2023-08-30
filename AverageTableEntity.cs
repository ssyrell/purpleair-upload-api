using System;
using Azure;
using Azure.Data.Tables;

namespace SteveSyrell.PurpleAirUploadApi
{
    public class AverageTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;

        public double TempFahrenheit { get; set; }

        public double Humidity { get; set; }

        public double DewpointFahrenheit { get; set; }

        public double Pressure { get; set; }

        public double TempFahrenheit680 { get; set; }

        public double Humidity680 { get; set; }

        public double DewpointFahrenheit680 { get; set; }

        public double Pressure680 { get; set; }

        public double Gas680 { get; set; }

        public int ChannelA_Pm25Aqi { get; set; }

        public double ChannelA_Pm10CF1 { get; set; }

        public double ChannelA_Pm25CF1 { get; set; }

        public double ChannelA_Pm100CF1 { get; set; }

        public double ChannelA_P03Um { get; set; }

        public double ChannelA_P05Um { get; set; }

        public double ChannelA_P10Um { get; set; }

        public double ChannelA_P25Um { get; set; }

        public double ChannelA_P50Um { get; set; }

        public double ChannelA_P100Um { get; set; }

        public double ChannelA_Pm10Atm { get; set; }

        public double ChannelA_Pm25Atm { get; set; }

        public double ChannelA_Pm100Atm { get; set; }

        public int ChannelB_Pm25Aqi { get; set; }

        public double ChannelB_Pm10CF1 { get; set; }

        public double ChannelB_Pm25CF1 { get; set; }

        public double ChannelB_Pm100CF1 { get; set; }

        public double ChannelB_P03Um { get; set; }

        public double ChannelB_P05Um { get; set; }

        public double ChannelB_P10Um { get; set; }

        public double ChannelB_P25Um { get; set; }

        public double ChannelB_P50Um { get; set; }

        public double ChannelB_P100Um { get; set; }

        public double ChannelB_Pm10Atm { get; set; }

        public double ChannelB_Pm25Atm { get; set; }

        public double ChannelB_Pm100Atm { get; set; }

        public AverageTableEntity Clone()
        {
            return new AverageTableEntity()
            {
                TempFahrenheit = TempFahrenheit,
                Humidity = Humidity,
                DewpointFahrenheit = DewpointFahrenheit,
                Pressure = Pressure,
                TempFahrenheit680 = TempFahrenheit680,
                Humidity680 = Humidity680,
                DewpointFahrenheit680 = DewpointFahrenheit680,
                Pressure680 = Pressure680,
                Gas680 = Gas680,
                ChannelA_Pm25Aqi = ChannelA_Pm25Aqi,
                ChannelA_Pm10CF1 = ChannelA_Pm10CF1,
                ChannelA_Pm25CF1 = ChannelA_Pm25CF1,
                ChannelA_Pm100CF1 = ChannelA_Pm100CF1,
                ChannelA_P03Um = ChannelA_P03Um,
                ChannelA_P05Um = ChannelA_P05Um,
                ChannelA_P10Um = ChannelA_P10Um,
                ChannelA_P25Um = ChannelA_P25Um,
                ChannelA_P50Um = ChannelA_P50Um,
                ChannelA_P100Um = ChannelA_P100Um,
                ChannelA_Pm10Atm = ChannelA_Pm10Atm,
                ChannelA_Pm25Atm = ChannelA_Pm25Atm,
                ChannelA_Pm100Atm = ChannelA_Pm100Atm,
                ChannelB_Pm25Aqi = ChannelB_Pm25Aqi,
                ChannelB_Pm10CF1 = ChannelB_Pm10CF1,
                ChannelB_Pm25CF1 = ChannelB_Pm25CF1,
                ChannelB_Pm100CF1 = ChannelB_Pm100CF1,
                ChannelB_P03Um = ChannelB_P03Um,
                ChannelB_P05Um = ChannelB_P05Um,
                ChannelB_P10Um = ChannelB_P10Um,
                ChannelB_P25Um = ChannelB_P25Um,
                ChannelB_P50Um = ChannelB_P50Um,
                ChannelB_P100Um = ChannelB_P100Um,
                ChannelB_Pm10Atm = ChannelB_Pm10Atm,
                ChannelB_Pm25Atm = ChannelB_Pm25Atm,
                ChannelB_Pm100Atm = ChannelB_Pm100Atm,
            };
        }
    }
}