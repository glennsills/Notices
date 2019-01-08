using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notices.EmailService
{
    public interface INoticeEmail
    {
        Task<string> SendNoticeEmail (string templateFilename,
            List<string> recipienents,
            Dictionary<string, string> parameters,
            string mandateArchiveFoldername);
    }
}