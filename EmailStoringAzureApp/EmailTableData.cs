using Azure;

namespace SnapBuild.EmailCaptureFunc
{
    public class EmailTableData : Azure.Data.Tables.ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        
        public string Html { get; set; }
        public ETag ETag { get; set; }
        public string Content { get; set; }

    }
}
