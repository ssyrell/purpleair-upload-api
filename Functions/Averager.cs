using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SteveSyrell.PurpleAirUploadApi.Entities;

namespace SteveSyrell.PurpleAirUploadApi
{
    public class Averager
    {
        /// <summary>
        /// Function to calculate averages of real-time data uploaded from purple air sensors.
        /// Function is invoked every 10 minutes and calculates averages of various windows 
        /// depending on the current time.
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("Averager")]
        public async Task Run([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var utilities = new Utilities();
            var thresholds = utilities.CalculateAveragerThresholds(DateTimeOffset.UtcNow);
            log.LogInformation($"[Averager] Calculating averages from {thresholds.End} to {thresholds.Start}.");

            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
            await this.CalculateAveragesAsync(tableServiceClient, thresholds, log);
        }

        private async Task CalculateAveragesAsync(TableServiceClient serviceClient, AveragerThresholds thresholds, ILogger log)
        {
            TableClient sensorTable = serviceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_SENSOR_TABLE_NAME"));
            TableClient realTimeTable = serviceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_REAL_TIME_TABLE_NAME"));

            var sensors = sensorTable.QueryAsync<SensorTableEntity>();
            await foreach (var sensor in sensors)
            {
                log.LogDebug($"Querying for sensor: {sensor.RowKey}");
                var sensorData = realTimeTable.QueryAsync<RealTimeTableEntity>(filter: $"PartitionKey eq '{sensor.RowKey}'");
                var sums = new AverageTableEntity();
                var rowCount = 0;
                var currentThreshold = this.GetNextThreshold(thresholds, thresholds.Start);
                await foreach (var row in sensorData)
                {
                    if (!currentThreshold.time.HasValue)
                    {
                        break;
                    }

                    if (row.Timestamp < currentThreshold.time)
                    {
                        var averages = this.DivideSums(sums, rowCount);
                        averages.PartitionKey = sensor.RowKey;
                        averages.RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - thresholds.Start.Ticks);
                        averages.Timestamp = thresholds.Start;
                        TableClient averageTable = serviceClient.GetTableClient(currentThreshold.tableName);
                        await averageTable.AddEntityAsync(averages);
                        currentThreshold = this.GetNextThreshold(thresholds, currentThreshold.time.Value);
                    }
                    else
                    {
                        this.AppendValues(sums, row);
                        rowCount++;
                    }
                }
            }
        }

        private (DateTimeOffset? time, string tableName) GetNextThreshold(AveragerThresholds thresholds, DateTimeOffset currentThreshold)
        {
            if (currentThreshold == thresholds.Start)
            {
                return (thresholds.TenMinutes, Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_TEN_MINUTE_AVERAGES_TABLE_NAME"));
            }

            if (currentThreshold == thresholds.TenMinutes)
            {
                return (thresholds.OneHour, Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_HOURLY_AVERAGES_TABLE_NAME"));
            }

            if (currentThreshold == thresholds.OneHour)
            {
                return (thresholds.OneDay, Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_DAILY_AVERAGES_TABLE_NAME"));
            }

            if (currentThreshold == thresholds.OneDay)
            {
                return (thresholds.OneMonth, Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_MONTHLY_AVERAGES_TABLE_NAME"));
            }

            return (null, string.Empty);
        }

        private void AppendValues(AverageTableEntity sums, RealTimeTableEntity values)
        {
            sums.TempFahrenheit += values.TempFahrenheit;
            sums.Humidity += values.Humidity;
            sums.DewpointFahrenheit += values.DewpointFahrenheit;
            sums.Pressure += values.Pressure;
            sums.TempFahrenheit680 += values.TempFahrenheit680;
            sums.Humidity680 += values.Humidity680;
            sums.DewpointFahrenheit680 += values.DewpointFahrenheit680;
            sums.Pressure680 += values.Pressure680;
            sums.Gas680 += values.Gas680;
            sums.ChannelA_Pm25Aqi += values.ChannelA_Pm25Aqi;
            sums.ChannelA_Pm10CF1 += values.ChannelA_Pm10CF1;
            sums.ChannelA_Pm25CF1 += values.ChannelA_Pm25CF1;
            sums.ChannelA_Pm100CF1 += values.ChannelA_Pm100CF1;
            sums.ChannelA_P03Um += values.ChannelA_P03Um;
            sums.ChannelA_P05Um += values.ChannelA_P05Um;
            sums.ChannelA_P10Um += values.ChannelA_P10Um;
            sums.ChannelA_P25Um += values.ChannelA_P25Um;
            sums.ChannelA_P50Um += values.ChannelA_P50Um;
            sums.ChannelA_P100Um += values.ChannelA_P100Um;
            sums.ChannelA_Pm10Atm += values.ChannelA_Pm10Atm;
            sums.ChannelA_Pm25Atm += values.ChannelA_Pm25Atm;
            sums.ChannelA_Pm100Atm += values.ChannelA_Pm100Atm;
            sums.ChannelB_Pm25Aqi += values.ChannelB_Pm25Aqi;
            sums.ChannelB_Pm10CF1 += values.ChannelB_Pm10CF1;
            sums.ChannelB_Pm25CF1 += values.ChannelB_Pm25CF1;
            sums.ChannelB_Pm100CF1 += values.ChannelB_Pm100CF1;
            sums.ChannelB_P03Um += values.ChannelB_P03Um;
            sums.ChannelB_P05Um += values.ChannelB_P05Um;
            sums.ChannelB_P10Um += values.ChannelB_P10Um;
            sums.ChannelB_P25Um += values.ChannelB_P25Um;
            sums.ChannelB_P50Um += values.ChannelB_P50Um;
            sums.ChannelB_P100Um += values.ChannelB_P100Um;
            sums.ChannelB_Pm10Atm += values.ChannelB_Pm10Atm;
            sums.ChannelB_Pm25Atm += values.ChannelB_Pm25Atm;
            sums.ChannelB_Pm100Atm += values.ChannelB_Pm100Atm;
        }

        private AverageTableEntity DivideSums(AverageTableEntity averages, int entryCount)
        {
            var result = averages.Clone();
            result.TempFahrenheit = result.TempFahrenheit / entryCount;
            result.Humidity = result.Humidity / entryCount;
            result.DewpointFahrenheit = result.DewpointFahrenheit / entryCount;
            result.Pressure = result.Pressure / entryCount;
            result.TempFahrenheit680 = result.TempFahrenheit680 / entryCount;
            result.Humidity680 = result.Humidity680 / entryCount;
            result.DewpointFahrenheit680 = result.DewpointFahrenheit680 / entryCount;
            result.Pressure680 = result.Pressure680 / entryCount;
            result.Gas680 = result.Gas680 / entryCount;
            result.ChannelA_Pm25Aqi = result.ChannelA_Pm25Aqi / entryCount;
            result.ChannelA_Pm10CF1 = result.ChannelA_Pm10CF1 / entryCount;
            result.ChannelA_Pm25CF1 = result.ChannelA_Pm25CF1 / entryCount;
            result.ChannelA_Pm100CF1 = result.ChannelA_Pm100CF1 / entryCount;
            result.ChannelA_P03Um = result.ChannelA_P03Um / entryCount;
            result.ChannelA_P05Um = result.ChannelA_P05Um / entryCount;
            result.ChannelA_P10Um = result.ChannelA_P10Um / entryCount;
            result.ChannelA_P25Um = result.ChannelA_P25Um / entryCount;
            result.ChannelA_P50Um = result.ChannelA_P50Um / entryCount;
            result.ChannelA_P100Um = result.ChannelA_P100Um / entryCount;
            result.ChannelA_Pm10Atm = result.ChannelA_Pm10Atm / entryCount;
            result.ChannelA_Pm25Atm = result.ChannelA_Pm25Atm / entryCount;
            result.ChannelA_Pm100Atm = result.ChannelA_Pm100Atm / entryCount;
            result.ChannelB_Pm25Aqi = result.ChannelB_Pm25Aqi / entryCount;
            result.ChannelB_Pm10CF1 = result.ChannelB_Pm10CF1 / entryCount;
            result.ChannelB_Pm25CF1 = result.ChannelB_Pm25CF1 / entryCount;
            result.ChannelB_Pm100CF1 = result.ChannelB_Pm100CF1 / entryCount;
            result.ChannelB_P03Um = result.ChannelB_P03Um / entryCount;
            result.ChannelB_P05Um = result.ChannelB_P05Um / entryCount;
            result.ChannelB_P10Um = result.ChannelB_P10Um / entryCount;
            result.ChannelB_P25Um = result.ChannelB_P25Um / entryCount;
            result.ChannelB_P50Um = result.ChannelB_P50Um / entryCount;
            result.ChannelB_P100Um = result.ChannelB_P100Um / entryCount;
            result.ChannelB_Pm10Atm = result.ChannelB_Pm10Atm / entryCount;
            result.ChannelB_Pm25Atm = result.ChannelB_Pm25Atm / entryCount;
            result.ChannelB_Pm100Atm = result.ChannelB_Pm100Atm / entryCount;

            return result;
        }
    }
}