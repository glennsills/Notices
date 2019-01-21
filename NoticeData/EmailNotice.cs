using System;

namespace Notices.NoticeData
{
    public class EmailNotice
    {
        public DateTime SendDateTime {get;set;} = DateTime.Now;

        public string EmailStorageName {get;set;}

        public string EmailAddress {get;set;}

        public bool IsReminder {get;set;}

        public bool IsResend {get;set;}
    }
}