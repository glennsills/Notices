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
    public class TestNoticeProvider : BaseNotifier, ITestNotifier
    {
        public TestNoticeProvider (ILogger<BaseNotifier> logger, INoticeEmail emailService, IDocumentService documentService) : 
            base (logger, emailService, documentService)
        { }


        public Task<NoticeRecord> Remind (string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAdderss)
        {
            throw new NotImplementedException ();
        }

        override public Task<PrincipalInformation> GetPrincipalInformationFromSource (string principalIdentifier, string purpose)
        {
            // Normall this information would come from a data source injected into the cstor

            var principalInfo = new PrincipalInformation
            {
                EmailAddresses = new List<string>{"someone@somewhere.com"},
                EmailParameters = new Dictionary<string,string>{
                     {"Link1HREF", "https://www.google.com"},
                     {"Link1Title", "Just Google It."},
                     {"Subject", "Consider just googling it."}
                },
                FormParameters = new Dictionary<string, string>{
                    {"Start of Calendar Year", "2019-01-01"},
                    {"End of Calendar Year", "2019-12-31"}
                },
                DocumentTemplate = "PaidSickLeave-MandatoryNotice-English.pdf",   
                EmailTemplate = "EmailTemplate.html"            
            };
            return Task.FromResult(principalInfo);
        }

        public Task<NoticeRecord> UploadNotification(string principalIdentifier, string emailAddress, string notificationFilePath, Mandate mandate, string purpose)
        {
            throw new NotImplementedException();
        }

        public override Task<DocumentRecord> CreateNotificationDocument(PrincipalInformation principalInfo)
        {
           return  _documentService.CreateNoticeDocument(principalInfo, Mandate.TestNotifications);
        }
    }
}