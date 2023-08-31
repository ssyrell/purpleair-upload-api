using System;
using Azure;
using Azure.Data.Tables;

namespace SteveSyrell.PurpleAirUploadApi
{
    public class SensorTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
}