using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using File = System.IO.File;

namespace FunctionApp1
{
    internal static class FileHelper
    {
        private const string DownloadFileUrl = "https://appsrg908a.blob.core.windows.net/public-files/example-spreadsheet.xlsx";

        internal static async Task UploadFileAsync(string fileName, ILogger log)
        {
            EnvironmentHelpers.GetAzureVariables(out string siteId, out string driveId,
             out string tenantId, out string clientId, out string clientSecret);

            EnvironmentHelpers.GetSteeringVariables(out int documentsCount, out int filesInSubatchCount,
             out int subBatchesCount, out int _);

            var authProvider = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var destinationFilePath = await SaveFile(fileName, log);
            await using var stream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read);
            try
            {
                var graphClient = new GraphServiceClient(authProvider);
                await UploadToMSGraph(fileName, siteId, driveId, stream, graphClient);
                var maxBatchIdx = GetMaxBatchIdx(documentsCount, filesInSubatchCount, subBatchesCount);
                log.LogInformation($"Uploaded: {destinationFilePath} from {BatchHelper.TaskIdentifier.Value} to {maxBatchIdx} ");
            }
            catch (Exception ex)
            {
                log.LogError($"Error uploading {destinationFilePath}: {ex.Message}");
            }
        }

        private static async Task<string> SaveFile(string fileName, ILogger log)
        {
            var destinationFilePath = Path.Combine(Path.GetTempPath(), fileName);

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(DownloadFileUrl);

            if (!response.IsSuccessStatusCode) { log.LogError("HTTP request failed with status code: " + response.StatusCode); }

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = File.Create(destinationFilePath);
            await contentStream.CopyToAsync(fileStream);

            log.LogWarning("File downloaded and saved as: " + destinationFilePath);
            return destinationFilePath;
        }

        private static int GetMaxBatchIdx(int documentsCount, int filesInSubatchCount, int subBatchesCount)
        {
            int max = filesInSubatchCount * subBatchesCount + BatchHelper.TaskIdentifier.Value - 1;
            int maxBatch = Math.Min(documentsCount, max);
            return maxBatch;
        }

        private static async Task UploadToMSGraph(string fileName, string siteId, string driveId, FileStream stream, GraphServiceClient graphClient)
        {
            await graphClient
                .Sites[siteId]
                .Drives[driveId]
                .Root
                .ItemWithPath(fileName)
                .Content
                .Request()
                .PutAsync<DriveItem>(stream);
        }
    }
}