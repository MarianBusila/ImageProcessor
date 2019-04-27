using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageProcessor
{
    public static class GetUploadUrl
    {
        [FunctionName("GetUploadUrl")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string filename = req.GetQueryParameterDictionary()
                .FirstOrDefault(q => string.Compare(q.Key, "filename", true) == 0)
                .Value;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING", EnvironmentVariableTarget.Process));
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("images");
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blob = container.GetBlockBlobReference(filename);

            SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
            };

            string sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);
            return new
            {
                url = blob.Uri + sasBlobToken
            };
        }
    }
}
