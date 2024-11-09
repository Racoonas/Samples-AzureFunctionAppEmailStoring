using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StrongGrid;

namespace SnapBuild.EmailCaptureFunc
{
    public class EmailCaptureFunction
    {
        private readonly ILogger<EmailCaptureFunction> _logger;

        public EmailCaptureFunction(ILogger<EmailCaptureFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(EmailCaptureFunction))]
        [TableOutput("Emails", Connection = "AzureWebJobsStorage")]
        public static async Task<EmailTableData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, 
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(EmailCaptureFunction));
            logger.LogInformation("EmailCaptureFunction. parsing request.");            
                        
            var parser = new WebhookParser();
            var inboundMail = await parser.ParseInboundEmailWebhookAsync(req.Body);

            var from = inboundMail.From.Email;
            var to = string.Join(", ", inboundMail.To.Select(t => t.Email));
            var subject = inboundMail.Subject;
            var text = inboundMail.Text;
            var html = inboundMail.Html;
            var content = inboundMail.RawEmail;

            var json = JsonConvert.SerializeObject(new {from, to, text, html, content}, Formatting.Indented);

            logger.LogInformation($"EmailCaptureFunction. Request parsed: {Environment.NewLine}{json}");

            logger.LogInformation("EmailCaptureFunction. Saving request to the table.");
            
            return new EmailTableData
            {
                PartitionKey = "EmailCapture",
                RowKey = Guid.NewGuid().ToString(),
                From = from,
                To = to,
                Subject = subject,
                Text = text,
                Html = html,
                Content = content,
                Timestamp = DateTime.Now
            };            
            
        }
    }
}
