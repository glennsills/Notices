namespace Notices.DocumentService
{
    public class DocumentRecord
    {
        public string DocumentName {get;set;}
        
        public bool WasSuccessful {get;set;}
        public string RequestKey { get; set; }
    }
}