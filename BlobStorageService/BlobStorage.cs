using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Notices.NoticeData;
using System.IO;
using System.Net;

namespace Notices.BlobStorageService
{
    public class BlobStorage : IBlobStorage
    {
        private BlobStorageOptions _options;
        private readonly ILogger<BlobStorage> _logger;

        public BlobStorage (IOptions<BlobStorageOptions> options, ILogger<BlobStorage> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task UploadFile (Mandate mandate, BlobUsage blobUsage, string sourceFilePath)
        {
            var container = await GetCloudContainer (mandate, blobUsage);
            var cloudBlockBlob = container.GetBlockBlobReference(GetBlobName(mandate, blobUsage,Path.GetFileName(sourceFilePath)));
            await cloudBlockBlob.UploadFromFileAsync(sourceFilePath);
        }

        private async Task<CloudBlobContainer> GetCloudContainer (Mandate mandate, BlobUsage blobUsage)
        {
            CloudStorageAccount.TryParse (_options.ConnectionString, out var storageAccount);

            var cloudBlobClient = storageAccount.CreateCloudBlobClient ();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference (_options.ApplicationName);
            await cloudBlobContainer.CreateIfNotExistsAsync();

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await cloudBlobContainer.SetPermissionsAsync (permissions);
            return cloudBlobContainer;
        }

        private string GetBlobName (Mandate mandate, BlobUsage blobUsage, string fileName)
        {
            return $"{mandate.ToString()}/{blobUsage.ToString()}/{fileName}" ;
            
        }
    }
}