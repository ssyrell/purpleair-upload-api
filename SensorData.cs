using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SteveSyrell.PurpleAirUploadApi
{
    public static class SensorData
    {
        [FunctionName("SensorData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("[SensorData] Processing upload request");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"[SensorData] Completed reading request payload - length={requestBody.Length}");

            var data = JsonSerializer.Deserialize<SensorDataTableEntity>(requestBody);
            data.PartitionKey = data.SensorId.ToLower();
            data.RowKey = string.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks);
            log.LogInformation("[SensorData] Deserialization complete");

            TableServiceClient tableServiceClient = new TableServiceClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING"));
            TableClient tableClient = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_TABLE_NAME"));

            await tableClient.AddEntityAsync(data);
            log.LogInformation("[SensorData] Data successfully added to table");

            return new OkResult();
        }
    }
}
