using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Notices.NoticeData;
using System.IO;
using System.Net;

namespace Notices.NoticeStorage
{
    public class NoticeStorageService : INoticeStorage
    {
        private NoticeStorageOptions _options;
        private readonly ILogger<NoticeStorageService> _logger;

        public NoticeStorageService (IOptions<NoticeStorageOptions> options, ILogger<NoticeStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task UploadFileFromStream (string fileName, Stream inputStream)
        {
            var container = await GetCloudContainer ();
            var cloudBlockBlob = container.GetBlockBlobReference(fileName);
            await cloudBlockBlob.UploadFromStreamAsync(inputStream);
        }

        public async Task<Stream> GetFileStream(string fileName)
        {
            var container = await GetCloudContainer ();
            var cloudBlockBlob = container.GetBlockBlobReference(fileName);
            return await cloudBlockBlob.OpenReadAsync();
        }

        public void SaveNoticeRecord(NoticeRecord noticeRecord)
        {
            throw new NotImplementedException();
        }

       
        private async Task<CloudBlobContainer> GetCloudContainer ()
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


    }
}