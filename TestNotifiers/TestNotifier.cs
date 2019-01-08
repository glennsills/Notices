using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;

namespace Notices.TestNotifiers
{
    public class TestNotifier : BaseNotifier, ITestNotifier
    {
        public TestNotifier (ILogger<BaseNotifier> logger, INoticeEmail emailService, IDocumentService documentService) : 
            base (logger, emailService, documentService)
        { }


        public Task<NoticeRecord> Remind (string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAdderss)
        {
            throw new NotImplementedException ();
        }

        override public Task<PrincipalInformation> GetPrincipalInformationFromSource (string principalIdentifier, string purpose)
        {
            var principalInfo = new PrincipalInformation
            {
                EmailAddresses = new List<string>{"someone@somewhere.com"},
                EmailParameters = new Dictionary<string,string>{
                     {"link1_href", "https://www.google.com"},
                     {"link1_text", "Just Google It."}
                },
                FormParameters = new Dictionary<string, string>{
                    {"Start of Calendar Year", "2019-01-01"},
                    {"End of Calendar Year", "2019-12-31"}
                },
                DocumentTemplate = "PaidSickLeave-MandatoryNotice-English.pdf"              
            };
            return Task.FromResult(principalInfo);
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