using System;

namespace Notices.NoticeData
{
    public class NoticeConfiguration
    {
        public string Id {get;set;}
        public DateTime StartingDate {get;set;}
        public DateTime EndingDate {get;set;}
        public EmailNoticeConfiguration[] EmailNoticeConfiguration {get;set;}
        public WebNoticeConfiguration[] WebNoticeConfiguration {get;set;}
    }
}
