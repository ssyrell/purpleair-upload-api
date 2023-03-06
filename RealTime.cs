using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SteveSyrell.PurpleAirUploadApi
{
    public static class RealTime
    {
        [FunctionName("RealTime")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("[RealTime] Processing upload request");
            var now = DateTime.UtcNow;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"[RealTime] Completed reading request payload - length={requestBody.Length}");

            var data = JsonSerializer.Deserialize<RealTimeTableEntity>(requestBody);
            log.LogInformation("[RealTime] Deserialization complete");

            // Order table rows using long tail pattern to allow 
            // for efficient fetching of most recent 'x' rows so
            // that we can quickly average data.
            var sensorId = data.SensorId;
            var invertedTicks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - now.Ticks);
            data.PartitionKey = data.SensorId;
            data.RowKey = invertedTicks;

            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
            TableClient realTimeTable = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_REAL_TIME_TABLE_NAME"));
            TableClient tenMinAveragesTable = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_TEN_MINUTE_AVERAGES_TABLE_NAME"));
            TableClient hourlyAveragesTable = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_HOURLY_AVERAGES_TABLE_NAME"));

            await realTimeTable.AddEntityAsync(data);
            log.LogInformation("[RealTime] Real-time data successfully added to table");

            // Purple air sensors upload data every 2 minutes, so for 1 hour of data, request the latest 30 rows.
            var query = realTimeTable.QueryAsync<RealTimeTableEntity>(filter: $"PartitionKey eq '{sensorId}'", maxPerPage: 30);
            var averages = await CalculateAveragesAsync(query);

            averages.tenMinAverages.PartitionKey = sensorId;
            averages.tenMinAverages.RowKey = invertedTicks;
            averages.hourlyAverages.PartitionKey = sensorId;
            averages.hourlyAverages.RowKey = invertedTicks;

            await Task.WhenAll(tenMinAveragesTable.AddEntityAsync(averages.tenMinAverages), hourlyAveragesTable.AddEntityAsync(averages.hourlyAverages));
            log.LogInformation("[RealTime] Averages data successfully added to average tables");

            log.LogInformation($"[RealTime] Completed processing upload request {(DateTime.UtcNow - now).TotalMilliseconds}ms");

            return new OkResult();
        }

        private static async Task<(AverageTableEntity tenMinAverages, AverageTableEntity hourlyAverages)> CalculateAveragesAsync(AsyncPageable<RealTimeTableEntity> query)
        {
            var rowCount = 0;
            var tenMinAverages = new AverageTableEntity();
            var hourlyAverages = new AverageTableEntity();
            await foreach (var row in query)
            {
                // Data is uploaded every 2 minutes, so the first 5 rows will contain the last 10 minutes of data.
                if (rowCount < 5)
                {
                    AppendValues(tenMinAverages, row);
                }

                AppendValues(hourlyAverages, row);
                rowCount++;

                if (rowCount == 30)
                {
                    break;
                }
            }

            DivideSums(tenMinAverages, Math.Min(5, rowCount));
            DivideSums(hourlyAverages, rowCount);

            return (tenMinAverages, hourlyAverages);
        }

        private static void AppendValues(AverageTableEntity sums, RealTimeTableEntity values)
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

        private static void DivideSums(AverageTableEntity averages, int entryCount)
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
