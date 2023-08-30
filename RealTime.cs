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
            log.LogInformation("[RealTime] Begin processing upload request");
            var now = DateTime.UtcNow;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<RealTimeTableEntity>(requestBody);
            log.LogInformation($"[RealTime] Received upload request for sensor {data.SensorId}");

            // Order table rows using long tail pattern to allow
            // for efficient fetching of most recent 'x' rows so
            // that we can quickly average data.
            var invertedTicks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - now.Ticks);
            data.PartitionKey = data.SensorId;
            data.RowKey = invertedTicks;

            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
            TableClient realTimeTable = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_REAL_TIME_TABLE_NAME"));

            await realTimeTable.AddEntityAsync(data);
            log.LogInformation($"[RealTime] Completed processing upload request for sensor {data.SensorId}. Total processing time: {(DateTime.UtcNow - now).TotalMilliseconds}ms");

            return new OkResult();
        }
    }
}
