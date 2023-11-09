using System;

namespace FunctionApp1
{
    internal static class EnvironmentHelpers
    {
        internal static void GetSteeringVariables(
            out int documentsCount, out int filesInSubatchCount,
            out int subBatchesCount, out int seccondsOfDelay)
        {
            documentsCount = int.TryParse(
                Environment.GetEnvironmentVariable("DocumentsCount"),
                 out int allDocumentsParsed)
                 ? allDocumentsParsed : 100;
            filesInSubatchCount = int.TryParse(
                Environment.GetEnvironmentVariable("BatchSize"),
                 out int batchSizeParsed)
                 ? batchSizeParsed : 10;
            subBatchesCount = int.TryParse(
                Environment.GetEnvironmentVariable("SubBatchSize"),
                out int subBatchSizeParsed)
                 ? subBatchSizeParsed : 3;
            seccondsOfDelay = int.TryParse(
                Environment.GetEnvironmentVariable("SeccondsOfDelay"),
                out int seccondsOfDelayParsed)
                ? seccondsOfDelayParsed : 15;
        }

        internal static void GetAzureVariables(out string siteId, out string driveId,
        out string tenantId, out string clientId, out string clientSecret)
        {
            siteId = Environment.GetEnvironmentVariable("SharePointOnline_SiteId") ?? string.Empty;
            driveId = Environment.GetEnvironmentVariable("SharePointOnline_DriveId") ?? string.Empty;
            tenantId = Environment.GetEnvironmentVariable("AzureAD_TenantId") ?? string.Empty;
            clientId = Environment.GetEnvironmentVariable("AzureAD_ClientId") ?? string.Empty;
            clientSecret = Environment.GetEnvironmentVariable("AzureAD_ClientSecret") ?? string.Empty;
        }
    }
}