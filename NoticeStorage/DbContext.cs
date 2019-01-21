using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Notices.NoticeStorage
{
    internal class DbContext
    {
        public DbContext()
        {

        }

        public async Task Initialize(string endpointUrl, string authorizationKey, string databaseId, string collectionId)
        {
            
            Client  = new DocumentClient(new Uri(endpointUrl), authorizationKey); 
            Database = Client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();
            if (Database == null)
            {
                Database = await Client.CreateDatabaseAsync(new Database { Id = databaseId });
            }
            DatabaseId = databaseId;
            
            Collection = await Client.CreateDocumentCollectionIfNotExistsAsync( UriFactory.CreateDatabaseUri(DatabaseId),
                    new DocumentCollection { Id = collectionId });

            CollectionId = collectionId;
        }
        public DocumentClient Client {get;set;}
        public Database Database {get;set;}
        public string DatabaseId {get;set;}

        public DocumentCollection Collection {get;set;}
        public string CollectionId { get; internal set; }
    }
}