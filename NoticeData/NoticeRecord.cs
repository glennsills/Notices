using System;
using System.Collections.Generic;

namespace Notices.NoticeData
{
    public class NoticeRecord
    {
        public string Id { get; set; }

        public bool WasSuccessful {get;set;}

        public string EmployeeIdentifier {get;set;}

        public string NoticeStorageName {get;set;}

        public string EmailStorageName {get;set;}

        public string ProcessMessage {get;set;}

        public string Mandate { get; set; }

        public DateTime NoticeDate { get; set; } = DateTime.Now.Date;

        public List<EmailNotice> EmailNotices {get;set;} = new List<EmailNotice>();
    }
}