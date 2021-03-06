using System.Collections.Generic;

namespace Notices.NoticeData
{
    public class ProcessingInformation
    {
        public List<string> EmailAddresses { get; set; }

        public Dictionary<string, string> EmailParameters { get; set; } = new Dictionary<string, string>();

        public List<EmailAttachment> EmailAttachments { get; set; } = new List<EmailAttachment>();

        public Dictionary<string, string> FormParameters { get; set; } = new Dictionary<string, string>();

        public string DocumentTemplate { get; set; }

        public string EmailTemplate { get; set; }

        public Mandate Mandate { get; set; }

    }
}