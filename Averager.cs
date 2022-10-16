using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;


namespace SteveSyrell.PurpleAirUploadApi
{
    public class Averager
    {
        [FunctionName("Averager")]
        public async Task Run([TimerTrigger("0 */10 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var now = DateTime.UtcNow;
            var isOnTheHour = now.Minute == 0;
            log.LogInformation($"[Averager] Invoking at {now}.");

            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
            TableClient historyTableClient = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_TABLE_NAME"));

            var tenMinQuery = historyTableClient.QueryAsync<SensorDataTableEntity>(x => x.Timestamp >= DateTimeOffset.UtcNow.AddMinutes(-10));
            var tenMinAverages = await this.CalculateAveragesAsync(tenMinQuery);


            TableClient tenMinAveragesTableClient = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_TEN_MINUTE_AVERAGES_TABLE_NAME"));
            foreach (var average in tenMinAverages)
            {
                await tenMinAveragesTableClient.AddEntityAsync(average);
            }

            if (isOnTheHour)
            {
                log.LogInformation("[Averager] Generating hourly averages, as this invocation occurs on the hour");
                var hourQuery = historyTableClient.QueryAsync<SensorDataTableEntity>(x => x.Timestamp >= DateTimeOffset.UtcNow.AddMinutes(-60));
                var hourlyAverages = (await this.CalculateAveragesAsync(hourQuery));
                TableClient hourlyAveragesTableClient = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_HOURLY_AVERAGES_TABLE_NAME"));
                foreach (var average in hourlyAverages)
                {
                    await hourlyAveragesTableClient.AddEntityAsync(average);
                }
            }

            log.LogInformation($"[Averager] Completed calculating averages in {(DateTime.UtcNow - now).TotalMilliseconds}ms");
        }

        private async Task<List<AverageTableEntity>> CalculateAveragesAsync(AsyncPageable<SensorDataTableEntity> query)
        {
            var results = new List<AverageTableEntity>();
            AverageTableEntity currentSensorAverages = null;
            var sensorEntryCount = 0;
            await foreach (var row in query)
            {
                if (currentSensorAverages == null)
                {
                    currentSensorAverages = new AverageTableEntity();
                    currentSensorAverages.PartitionKey = row.SensorId;
                }

                if (row.SensorId != currentSensorAverages.PartitionKey && sensorEntryCount > 0)
                {
                    this.DivideSums(currentSensorAverages, sensorEntryCount);
                    currentSensorAverages.RowKey = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
                    results.Add(currentSensorAverages);

                    currentSensorAverages = new AverageTableEntity();
                    currentSensorAverages.PartitionKey = row.SensorId;
                    sensorEntryCount = 0;
                }

                AppendValues(currentSensorAverages, row);
                sensorEntryCount++;
            }

            // Add the last average row to the list
            this.DivideSums(currentSensorAverages, sensorEntryCount);
            currentSensorAverages.RowKey = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            results.Add(currentSensorAverages);

            return results;
        }

        private void AppendValues(AverageTableEntity sums, SensorDataTableEntity values)
        {
            sums.TempFahrenheit += values.CurrentTempFahrenheit;
            sums.Humidity += values.CurrentHumidity;
            sums.DewpointFahrenheit += values.CurrentDewpointFahrenheit;
            sums.Pressure += values.Pressure;
            sums.TempFahrenheit680 += values.CurrentTempFahrenheit680;
            sums.Humidity680 += values.CurrentHumidity680;
            sums.DewpointFahrenheit680 += values.CurrentDewpointFahrenheit680;
            sums.Pressure680 += values.Pressure680;
            sums.Gas680 += values.Gas680;
            sums.Pm25Aqi += (values.ChannelA_Pm25Aqi + values.ChannelB_Pm25Aqi) / 2;
            sums.Pm10CF1 += (values.ChannelA_Pm10CF1 + values.ChannelB_Pm10CF1) / 2;
            sums.Pm25CF1 += (values.ChannelA_Pm25CF1 + values.ChannelB_Pm25CF1) / 2;
            sums.Pm100CF1 += (values.ChannelA_Pm100CF1 + values.ChannelB_Pm100CF1) / 2;
            sums.P03Um += (values.ChannelA_P03Um + values.ChannelB_P03Um) / 2;
            sums.P05Um += (values.ChannelA_P05Um + values.ChannelB_P05Um) / 2;
            sums.P10Um += (values.ChannelA_P10Um + values.ChannelB_P10Um) / 2;
            sums.P25Um += (values.ChannelA_P25Um + values.ChannelB_P25Um) / 2;
            sums.P50Um += (values.ChannelA_P50Um + values.ChannelB_P50Um) / 2;
            sums.P100Um += (values.ChannelA_P100Um + values.ChannelB_P100Um) / 2;
            sums.Pm10Atm += (values.ChannelA_Pm10Atm + values.ChannelB_Pm10Atm) / 2;
            sums.Pm25Atm += (values.ChannelA_Pm25Atm + values.ChannelB_Pm25Atm) / 2;
            sums.Pm100Atm += (values.ChannelA_Pm100Atm + values.ChannelB_Pm100Atm) / 2;
        }

        private void DivideSums(AverageTableEntity averages, int entryCount)
        {
            averages.TempFahrenheit = averages.TempFahrenheit / entryCount;
            averages.Humidity = averages.Humidity / entryCount;
            averages.DewpointFahrenheit = averages.DewpointFahrenheit / entryCount;
            averages.Pressure = averages.Pressure / entryCount;
            averages.TempFahrenheit680 = averages.TempFahrenheit680 / entryCount;
            averages.Humidity680 = averages.Humidity680 / entryCount;
            averages.DewpointFahrenheit680 = averages.DewpointFahrenheit680 / entryCount;
            averages.Pressure680 = averages.Pressure680 / entryCount;
            averages.Gas680 = averages.Gas680 / entryCount;
            averages.Pm25Aqi = averages.Pm25Aqi / entryCount;
            averages.Pm10CF1 = averages.Pm10CF1 / entryCount;
            averages.Pm25CF1 = averages.Pm25CF1 / entryCount;
            averages.Pm100CF1 = averages.Pm100CF1 / entryCount;
            averages.P03Um = averages.P03Um / entryCount;
            averages.P05Um = averages.P05Um / entryCount;
            averages.P10Um = averages.P10Um / entryCount;
            averages.P25Um = averages.P25Um / entryCount;
            averages.P50Um = averages.P50Um / entryCount;
            averages.P100Um = averages.P100Um / entryCount;
            averages.Pm10Atm = averages.Pm10Atm / entryCount;
            averages.Pm25Atm = averages.Pm25Atm / entryCount;
            averages.Pm100Atm = averages.Pm100Atm / entryCount;
        }
    }
}
