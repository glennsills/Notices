namespace Notices.EmailService
{
    public class NoticeEmailOptions
    {
        public string EmailTemplateFolder {get;set;}
        public string EmailSenderAddress {get;set;}
        public string EmailSenderName {get;set;}
        public string EmailArchiveFolder {get;set;}
        public string SmtpHost { get; internal set; }
        public int SmtpPort { get; internal set; }
    }
}