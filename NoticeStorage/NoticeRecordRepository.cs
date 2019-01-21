using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Notices.NoticeData;

namespace Notices.NoticeStorage
{
    public class NoticeRecordRepository
    {

        private readonly string _endpointUrl;
        private readonly string _authorizationKey;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly DbContext _dbContext;

        public NoticeRecordRepository (IOptions<NoticeStorageOptions> options)
        {
            _endpointUrl = options.Value.EndpointUrl;
            _authorizationKey = options.Value.AuthorizationKey;
            _databaseId = options.Value.DatabaseId;
            _collectionId = options.Value.CollectionId;
            _dbContext = new DbContext ();
            _dbContext.Initialize (_endpointUrl, _authorizationKey, _databaseId, _collectionId).Wait ();

        }

        public async Task<bool> SaveRecord (NoticeRecord record)
        {
            var response = await _dbContext.Client.CreateDocumentAsync (
                UriFactory.CreateDocumentCollectionUri (_dbContext.DatabaseId, _dbContext.CollectionId), record);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            return false;
        }
    }
}