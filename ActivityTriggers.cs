using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using FunctionApp1.Models;

namespace FunctionApp1
{
    public static class ActivityTriggers
    {
        [FunctionName(nameof(BatchProcessor))]
        public static async Task BatchProcessor([ActivityTrigger] DataWithIndex my, ILogger log)
        {
            BatchHelper.TaskIdentifier.Value = my.Idx;
            var tasks = new List<Task>();
            foreach (var name in my.Names)
            {
                tasks.Add(FileHelper.UploadFileAsync(name, log));
            }
            await Task.WhenAll(tasks);
        }
    }
}
