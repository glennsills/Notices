namespace Notices.NoticeService
{
    public class NoticeRecord
    {
        public bool WasSuccessful {get;set;}

        public string EmployeeIdentifier {get;set;}

        public string NoticeFilename {get;set;}

        public string EmailArchiveFilename {get;set;}

        public string ProcessMessage {get;set;}
    }
}