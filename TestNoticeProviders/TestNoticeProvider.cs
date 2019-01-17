using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Notices.DocumentService;
using Notices.EmailService;
using Notices.NoticeData;
using Notices.NoticeService;
using Notices.NoticeStorage;

namespace Notices.TestNotifiers
{
    public class TestNoticeProvider : BaseNoticeProvider, ITestNotifier
    {
        public TestNoticeProvider (ILogger<BaseNoticeProvider> logger, INoticeEmail emailService, IDocumentService documentService, INoticeStorage storageService) : 
            base (logger, emailService, documentService, storageService)
        { }


        public Task<NoticeRecord> Remind (string employeeIdentifier, Mandate mandate, Guid notificationId, string alternateEmailAdderss)
        {
            throw new NotImplementedException ();
        }

        override public Task<ProcessingInformation> GetProcessingInformationFromSource (string principalIdentifier, string purpose)
        {
            // Normall this information would come from a data source injected into the cstor

            var principalInfo = new ProcessingInformation
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

        public override Task<DocumentRecord> CreateNotificationDocument(ProcessingInformation principalInfo)
        {
           return  _documentService.CreateNoticeDocument(principalInfo, Mandate.TestNotifications);
        }
    }
}