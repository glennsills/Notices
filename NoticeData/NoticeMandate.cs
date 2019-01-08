namespace Notices.NoticeData
{
    public class NoticeMandate
    {
        public string id {get;set;}
        public string Organization {get;set;}
        public NoticeConfiguration[] Configurations {get;set;}
    }
}