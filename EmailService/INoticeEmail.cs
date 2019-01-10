using System.Collections.Generic;
using System.Threading.Tasks;
using Notices.NoticeData;

namespace Notices.EmailService
{
    public interface INoticeEmail
    {
        Task<string> SendNoticeEmail (string templateFilename,
            List<string> recipienents,
            Dictionary<string, string> parameters,
            Mandate mandateArchiveFoldername);
        Task<string> SendNoticeEmailWithAttachments(
            string emailTemplate, 
            List<string> emailAddresses, 
            Dictionary<string, string> emailParameters, 
            Mandate madate,
            List<string> attachmentFiles);
    }
}