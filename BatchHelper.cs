using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionApp1
{
    internal static class BatchHelper
    {
        internal static readonly AsyncLocal<int> TaskIdentifier = new();

        internal static IEnumerable<IEnumerable<IEnumerable<string>>> CreateBatches(int documentsCount, int filesInSubatchCount, int subBatchesCount)
        {
            var batches = new List<IEnumerable<IEnumerable<string>>>();
            for (int batchNo = 0;
             batchNo < documentsCount;
             batchNo += filesInSubatchCount * subBatchesCount)
            {
                var batch = BatchHelper.CreateBatch(documentsCount, filesInSubatchCount, subBatchesCount, batchNo);
                batches.Add(batch);
            }

            return batches;
        }

        private static IEnumerable<IEnumerable<string>> CreateBatch(int documentsCount,
        int filesInSubatchCount, int subBatchesCount, int batchNo)
        {
            var subBatches = new List<IEnumerable<string>>();
            for (int subBatchNo = 0; subBatchNo < subBatchesCount
             && batchNo + subBatchNo * filesInSubatchCount < documentsCount;
             subBatchNo++)
            {
                var subBatch = CreateSubBatch(documentsCount, filesInSubatchCount, batchNo, subBatchNo);
                subBatches.Add(subBatch);
            }
            return subBatches;
        }

        private static IEnumerable<string> CreateSubBatch(int documentsCount,
        int filesInSubatchCount, int batchNo, int subBatchNo)
        {
            var subBatch = new List<string>();
            for (int fileInSubBatchNo = 0; fileInSubBatchNo < filesInSubatchCount
              && batchNo + fileInSubBatchNo + subBatchNo * filesInSubatchCount < documentsCount;
             fileInSubBatchNo++)
            {
                var fileNo = batchNo + fileInSubBatchNo + subBatchNo * filesInSubatchCount;
                subBatch.Add($"File{fileNo}-SubBatch{subBatchNo}-fileInSubBatch{fileInSubBatchNo}.xlsx");
            }
            return subBatch;
        }

    }
}
