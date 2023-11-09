using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;
using FunctionApp1.Models;

namespace FunctionApp1
{
    public static class OrchestrationTriggers
    {
        [FunctionName(nameof(RunOrchestrator))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            await OptimalApproach(context);
        }

        private static async Task OptimalApproach(IDurableOrchestrationContext context)
        {
            EnvironmentHelpers.GetSteeringVariables(out int documentsCount, out int filesInSubatchCount, out int subBatchesCount, out int seccondsOfDelay);
            var batches = BatchHelper.CreateBatches(documentsCount, filesInSubatchCount, subBatchesCount);
            await ExecuteBatches(context, filesInSubatchCount, subBatchesCount, seccondsOfDelay, batches);
        }

        private static async Task ExecuteBatches(IDurableOrchestrationContext context, int batchSize,
          int subBatchSize, int seccondsOfDelay, IEnumerable<IEnumerable<IEnumerable<string>>> batches)
        {
            int idx = 1;
            foreach (var batch in batches)
            {
                await ExecuteSubBatches(context, batchSize, subBatchSize, seccondsOfDelay, idx, batch);
                idx += subBatchSize * batchSize;
            }
        }

        private static async Task ExecuteSubBatches(IDurableOrchestrationContext context, int batchSize,
         int subBatchSize, int seccondsOfDelay, int idx, IEnumerable<IEnumerable<string>> batch)
        {
            foreach (var subBatch in batch)
            {
                var dataWithIndex = new DataWithIndex(subBatch, idx);
                await context.CallActivityAsync(nameof(ActivityTriggers.BatchProcessor), dataWithIndex);
            }
            await DelayExecution(context, seccondsOfDelay);
        }

        private static async Task DelayExecution(IDurableOrchestrationContext context, int seccondsOfDelay)
        {
            DateTime dueTime = context.CurrentUtcDateTime.AddSeconds(seccondsOfDelay);
            await context.CreateTimer(dueTime, CancellationToken.None);
        }
    }
}

