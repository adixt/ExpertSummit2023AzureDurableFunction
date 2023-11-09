using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public static class HttpTriggers
    {
        [FunctionName(nameof(HttpStart))]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var instanceId = await starter.StartNewAsync(nameof(OrchestrationTriggers.RunOrchestrator));
            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}