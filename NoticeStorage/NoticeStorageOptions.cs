namespace Notices.NoticeStorage
{
    public class NoticeStorageOptions
    {
        public string ConnectionString {get;set;}
        public string ApplicationName {get;set;}
        public string EndpointUrl { get; set; }
        public string AuthorizationKey { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
    }
}