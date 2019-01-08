using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;

namespace Notices.DCNotifier
{
    public class DCWageNotifier : BaseNotifier, IDCWageNotifier
    {

        public DCWageNotifier(ILogger<BaseNotifier> logger, INoticeEmail emailService): base(logger, emailService){}

        public Task<NoticeRecord> Notify(string employeeIdentifier, Mandate mandate, string purpose)
        {
            throw new NotImplementedException();
        }

        public Task<NoticeRecord> Remind(string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAddress)
        {
            throw new NotImplementedException();
        }

        override public Task<PrincipalInformation> GetPrincipalInformationFromSource(string principalIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<NoticeRecord> UploadNotification(string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose)
        {
            throw new NotImplementedException();
        }

        public override Task<DocumentRecord> CreateNotificationDocument(PrincipalInformation principalInfo)
        {
            throw new NotImplementedException();
        }
    }
}
